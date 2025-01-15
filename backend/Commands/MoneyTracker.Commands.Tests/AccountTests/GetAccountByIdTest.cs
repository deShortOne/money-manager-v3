
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.AccountTests;
class GetAccountByIdTest : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IAccountCommandRepository _accountRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _accountRepo = new AccountCommandRepository(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void GetAccountId1()
    {
        Assert.Equal(new AccountEntity(1, "bank a", 1), await _accountRepo.GetAccountById(1));
    }

    [Fact]
    public async void GetAccountId2()
    {
        Assert.Equal(new AccountEntity(2, "bank b", 1), await _accountRepo.GetAccountById(2));
    }

    [Fact]
    public async void GetAccountId3()
    {
        Assert.Equal(new AccountEntity(3, "bank a", 2), await _accountRepo.GetAccountById(3));
    }

    [Fact]
    public async void FailToGetInvalidAccount()
    {
        Assert.Null(await _accountRepo.GetAccountById(-1));
    }
}
