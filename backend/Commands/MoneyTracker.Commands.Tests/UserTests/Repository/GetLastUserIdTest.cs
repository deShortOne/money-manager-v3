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
public sealed class GetlastUserIdTest : IAsyncLifetime
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

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _userRepo = new UserCommandRepository(_database, Mock.Of<IDateTimeProvider>());
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task GetLastUserIdWithNoDataInDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        Assert.Equal(0, await _userRepo.GetLastUserId());
    }

    [Fact]
    public async Task GetLastUserIdWithDataInDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Equal(2, await _userRepo.GetLastUserId());
    }
}
