using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.PostgresDb;
public sealed class GetAccountsTest : IClassFixture<PostgresDbFixture>
{
    private readonly PostgresDbFixture _postgresFixture;

    public GetAccountsTest(PostgresDbFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThere()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var accountDb = new AccountDatabase(db);

        var actual = await accountDb.GetAccounts(new AuthenticatedUser(1));

        var expected = new List<AccountEntity>()
        {
            new(1, "bank a"),
            new(2, "bank b"),
        };

        Assert.Equal(expected, actual);
    }
}
