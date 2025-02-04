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
public sealed class GetUserFromTokenTest : IAsyncLifetime
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
    public async Task SuccessfullyLogInUser()
    {
        var userId = 28934;
        var token = Guid.NewGuid();
        var expires = new DateTime(2024, 10, 6, 13, 00, 00, DateTimeKind.Utc);
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2024, 6, 5, 0, 0, 0));
        var expected = new UserAuthentication(new UserEntity(userId, "a", "b"), token.ToString(),
            expires, dateTimeProvider.Object);

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

        var queryInsertTokenForUser = """
            INSERT INTO user_id_to_token VALUES
            (@userId, @token, @expire)
         """;
        var queryInsertTokenForUserParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", userId),
            new NpgsqlParameter("token", token),
            new NpgsqlParameter("expire", expires),
        };
        await _database.UpdateTable(queryInsertTokenForUser, queryInsertTokenForUserParams); // Insert user token

        var actual = await _userAuthRepo.GetUserAuthFromToken(token.ToString());
        Assert.Equal(expected, actual);
    }


    [Fact]
    public async Task ReturnNullForIncorrectToken()
    {
        var userId = 28934;
        var token = Guid.NewGuid().ToString();
        var expires = new DateTime(2024, 10, 6, 13, 00, 00, DateTimeKind.Utc);

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

        var queryInsertTokenForUser = """
            INSERT INTO user_id_to_token VALUES
            (@userId, @token, @expire)
         """;
        var queryInsertTokenForUserParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", userId),
            new NpgsqlParameter("token", token),
            new NpgsqlParameter("expire", expires),
        };
        await _database.UpdateTable(queryInsertTokenForUser, queryInsertTokenForUserParams); // Insert user token

        var actual = await _userAuthRepo.GetUserAuthFromToken(Guid.NewGuid().ToString());
        Assert.Null(actual);
    }
}
