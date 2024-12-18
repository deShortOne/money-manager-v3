using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.AuthenticationTests.Repository;
public sealed class StoreTokenForUserTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    private IUserAuthRepository _userAuthRepo;
    private IDatabase _database;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _userAuthRepo = new UserAuthRepository(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void StoreTokenForUserSuccessfully()
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

        await _userAuthRepo.StoreTemporaryTokenToUser(new AuthenticatedUser(userId), token, expiration);

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
            new NpgsqlParameter("token", token),
            new NpgsqlParameter("expiration", expiration),
        };
        var reader = await _database.GetTable(query, queryParams);
        Assert.True(reader.HasRows);
    }
}
