using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.UserTests.Repository;
public sealed class StoreTokenForUserTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    private UserCommandRepository _userAuthRepo;
    private IDatabase _database;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _userAuthRepo = new UserCommandRepository(_database, Mock.Of<IDateTimeProvider>());
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task StoreTokenForUserSuccessfully()
    {
        var userId = 5623;
        var token = Guid.NewGuid();
        var expiration = new DateTime(2024, 10, 6, 15, 0, 0, DateTimeKind.Utc);

        var queryInsertUser = """
            INSERT INTO users VALUES (@id, @name, @password);
            """;
        var queryInsertUserParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", userId),
            new NpgsqlParameter("name", "a"),
            new NpgsqlParameter("password", "b"),
        };
        await _database.UpdateTable(queryInsertUser, queryInsertUserParams); // Insert user

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2024, 6, 5, 0, 0, 0));
        await _userAuthRepo.StoreTemporaryTokenToUser(new UserAuthentication(new UserEntity(userId, "", ""), token.ToString(),
            expiration, dateTimeProvider.Object));

        var query = """
            SELECT 1
            FROM user_id_to_token
            WHERE user_id = @id
            AND token = @token
            AND expires = @expiration;
        """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", userId),
            new NpgsqlParameter("token", token.ToString()),
            new NpgsqlParameter("expiration", expiration),
        };
        var reader = await _database.GetTable(query, queryParams);
        Assert.True(reader.Rows.Count != 0);
    }
}
