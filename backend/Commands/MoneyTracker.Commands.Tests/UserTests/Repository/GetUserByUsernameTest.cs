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

namespace MoneyTracker.Tests.AuthenticationTests.Repository;
public sealed class GetUserByUsernameTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    private UserCommandRepository _userRepo;
    private IDatabase _database;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _userRepo = new UserCommandRepository(_database, Mock.Of<IDateTimeProvider>());
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task GetUserByUsernameSuccessfully()
    {
        var userId = 5623;
        var username = "bob";
        var password = "scree";
        var expected = new UserEntity(userId, username, password);

        var queryInsertUser = """
            INSERT INTO users VALUES (@id, @name, @password);
            """;
        var queryInsertUserParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", userId),
            new NpgsqlParameter("name", username),
            new NpgsqlParameter("password", password),
        };
        await _database.UpdateTable(queryInsertUser, queryInsertUserParams); // Insert user

        var actual = await _userRepo.GetUserByUsername(username);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserByUsernameFailDueToNameNotBeingInDatabase()
    {
        var actual = await _userRepo.GetUserByUsername("asd");
        Assert.Null(actual);
    }
}
