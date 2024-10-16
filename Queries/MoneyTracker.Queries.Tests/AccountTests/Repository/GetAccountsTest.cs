using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.OldTestsToMoveOver.Core;
public sealed class GetAccountsTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        return;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThere()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var accountDb = new AccountRepository(db);
        var accountService = new AccountService(accountDb);

        var actual = await accountService.GetAccounts(new AuthenticatedUser(1));

        var expected = new List<AccountResponse>()
        {
            new(1, "bank a"),
            new(2, "bank b"),
        };

        Assert.Equal(expected, actual);
    }
}
