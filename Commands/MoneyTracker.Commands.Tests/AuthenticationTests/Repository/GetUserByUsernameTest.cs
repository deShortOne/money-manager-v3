using System.Data.Common;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
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

#pragma warning disable CS8618 // disable nullable
    private IUserAuthRepository _userAuthRepo;
    private IDatabase _database;
#pragma warning restore CS8618

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
    public async void GetUserByUsernameSuccessfully()
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

        var actual = await _userAuthRepo.GetUserByUsername(username);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void GetUserByUsernameFailDueToNameNotBeingInDatabase()
    {
        var actual = await _userAuthRepo.GetUserByUsername("asd");
        Assert.Null(actual);
    }
}
