using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Authentication.Tests.Repository;
public sealed class StoreTokenForUserTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

#pragma warning disable CS8618 // disable nullable
    private IUserAuthDatabase _userAuthRepo;
    private IDatabase _database;
#pragma warning restore CS8618

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _userAuthRepo = new UserAuthDatabase(_database);
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
