using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;
using Testcontainers.MongoDb;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.MongoDb;
public sealed class SaveAndGetAccountsTest : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public SaveAndGetAccountsTest(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var accountCache = new AccountCache(mongoDb);

        var authedUser = new AuthenticatedUser(2);
        var accounts = new List<AccountEntity>
        {
            new(23, "mdgnsrblya"),
            new(39, "sncjrckybz"),
            new(8, "cptcnvravt"),
        };

        await accountCache.SaveAccounts(authedUser, accounts);

        var result = await accountCache.GetAccounts(authedUser);

        Assert.Equal(accounts, result);
    }


    [Fact]
    public async Task EmptyAsDifferentUserAttemptsToAccessInformation()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var accountCache = new AccountCache(mongoDb);

        var authedUser1 = new AuthenticatedUser(2);
        var authedUser2 = new AuthenticatedUser(3);

        var accounts = new List<AccountEntity>
        {
            new(23, "mdgnsrblya"),
            new(39, "sncjrckybz"),
            new(8, "cptcnvravt"),
        };

        await accountCache.SaveAccounts(authedUser1, accounts);

        var result = await accountCache.GetAccounts(authedUser2);

        Assert.False(result.IsSuccess);
    }
}
