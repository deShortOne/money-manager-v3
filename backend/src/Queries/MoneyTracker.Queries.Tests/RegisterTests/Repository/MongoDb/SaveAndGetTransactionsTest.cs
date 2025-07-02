using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;
using Testcontainers.MongoDb;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.MongoDb;
public sealed class SaveAndGetCategoriesTest : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public SaveAndGetCategoriesTest(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var registerCache = new RegisterCache(mongoDb);

        var authedUser = new AuthenticatedUser(523);
        var transactions = new List<TransactionEntity>
        {
            new(1, 2, "payee name", 3, new DateOnly(), 4, "category name", 5, "payer name"),
            new(665, 116, "rNmcgjGvwu", 145, new DateOnly(), 473, "EzJENnGFhM", 100, "OyjFiDbFUZ"),
            new(360, 523, "ABZsTGUygM", 477, new DateOnly(), 954, "JhNpucfVNM", 767, "WDpsTOHItH"),
        };

        await registerCache.SaveTransactions(authedUser, transactions);

        var result = await registerCache.GetAllTransactions(authedUser, CancellationToken.None);

        Assert.Equal(transactions, result);
    }

    [Fact]
    public async Task EmptyAsDifferentUserAttemptsToAccessInformation()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var registerCache = new RegisterCache(mongoDb);

        var authedUser1 = new AuthenticatedUser(523);
        var authedUser2 = new AuthenticatedUser(979);
        var transactions = new List<TransactionEntity>
        {
            new(1, 2, "payee name", 3, new DateOnly(), 4, "category name", 5, "payer name"),
            new(665, 116, "rNmcgjGvwu", 145, new DateOnly(), 473, "EzJENnGFhM", 100, "OyjFiDbFUZ"),
            new(360, 523, "ABZsTGUygM", 477, new DateOnly(), 954, "JhNpucfVNM", 767, "WDpsTOHItH"),
        };

        await registerCache.SaveTransactions(authedUser1, transactions);

        var result = await registerCache.GetAllTransactions(authedUser2, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
