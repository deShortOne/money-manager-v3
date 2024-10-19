using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Moq;
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
        var token = "ASDFDSA";

        var mockAuthToken = new Mock<IUserAuthenticationService>();
        mockAuthToken.Setup(x => x.DecodeToken(token)).ReturnsAsync(new AuthenticatedUser(1));

        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var accountDb = new AccountRepository(db);
        var accountService = new AccountService(mockAuthToken.Object, accountDb);

        var actual = await accountService.GetAccounts(token);

        var expected = new List<AccountResponse>()
        {
            new(1, "bank a"),
            new(2, "bank b"),
        };

        Assert.Equal(expected, actual);
    }
}
