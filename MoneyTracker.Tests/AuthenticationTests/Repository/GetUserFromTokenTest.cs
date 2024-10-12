using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.AuthenticationTests.Repository;
public sealed class GetUserFromTokenTest : IAsyncLifetime
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
    public async void SuccessfullyLogInUser()
    {
        var userId = 28934;
        var token = Guid.NewGuid();
        var expires = new DateTime(2024, 10, 6, 13, 00, 00, DateTimeKind.Utc);
        var expected = new TokenMapToUserDTO(userId, expires);

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

        var actual = await _userAuthRepo.GetUserFromToken(token);
        Assert.Equal(expected, actual);
    }


    [Fact]
    public async void ReturnNullForIncorrectToken()
    {
        var userId = 28934;
        var token = Guid.NewGuid();
        var expires = new DateTime(2024, 10, 6, 13, 00, 00, DateTimeKind.Utc);
        var expected = new TokenMapToUserDTO(userId, expires);

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

        var actual = await _userAuthRepo.GetUserFromToken(Guid.NewGuid());
        Assert.Null(actual);
    }
}
