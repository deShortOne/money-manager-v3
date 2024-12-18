using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.AccountTests.Repository;
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

        var actual = await accountDb.GetAccounts(new AuthenticatedUser(1));

        var expected = new List<AccountEntity>()
        {
            new(1, "bank a"),
            new(2, "bank b"),
        };

        Assert.Equal(expected, actual);
    }
}
