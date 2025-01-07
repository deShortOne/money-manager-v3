
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.AccountTests;
public class IsValidAccountTest : IAsyncLifetime
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
    public void ValidAccounts()
    {

        Assert.Multiple(async () =>
        {
            Assert.True(await _accountRepo.IsValidAccount(1));
            Assert.True(await _accountRepo.IsValidAccount(5));
        });
    }

    [Fact]
    public void AccountsDoNotExist()
    {
        Assert.Multiple(async () =>
        {
            Assert.False(await _accountRepo.IsValidAccount(-1));
            Assert.False(await _accountRepo.IsValidAccount(0));
        });
    }
}
