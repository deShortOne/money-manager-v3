
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.AccountTests;
public class IsAccountOwnedByUserTest : IAsyncLifetime
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
    public async void AccountDoesBelongToUser()
    {
        Assert.True(await _accountRepo.IsAccountOwnedByUser(new Authentication.DTOs.AuthenticatedUser(1), 1));
    }

    [Fact]
    public async void AccountDoesNotBelongToUser()
    {
        Assert.False(await _accountRepo.IsAccountOwnedByUser(new Authentication.DTOs.AuthenticatedUser(1), 3));
    }

    [Fact]
    public async void AccountDoesNotExist()
    {
        Assert.False(await _accountRepo.IsAccountOwnedByUser(new Authentication.DTOs.AuthenticatedUser(1), 15));
    }
}
