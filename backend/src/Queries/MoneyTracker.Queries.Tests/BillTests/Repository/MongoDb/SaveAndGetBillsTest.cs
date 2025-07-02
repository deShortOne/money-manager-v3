using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.MongoDb;
public sealed class SaveAndGetBillsTest : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public SaveAndGetBillsTest(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    [Fact]
    public async Task Test()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var billCache = new BillCache(mongoDb);

        var authedUser = new AuthenticatedUser(2);
        var bills = new List<BillEntity>
        {
            new(1, 2, "payee name", 3, new DateOnly(), 4, "frequency name", 5, "category name", 6, "payer name"),
            new(62, 398, "TMCRaKglDO", 438, new DateOnly(), 797, "rshpoRRnmx", 713, "AHEqoPvGVx", 185, "liXwifejtC"),
            new(479, 259, "OAYEzcsXAu", 942, new DateOnly(), 436, "HOWUSFkJLd", 947, "NBOGcXgiXW", 91, "alGdvJZVPh"),
        };

        await billCache.SaveBills(authedUser, bills);

        var result = await billCache.GetAllBills(authedUser, CancellationToken.None);

        Assert.Equal(bills, result);
    }

    [Fact]
    public async Task EmptyAsDifferentUserAttemptsToAccessInformation()
    {
        var mongoDb = new MongoDatabase(_mongoDbFixture.ConnectionString);
        var billCache = new BillCache(mongoDb);

        var authedUser1 = new AuthenticatedUser(2);
        var authedUser2 = new AuthenticatedUser(3);
        var bills = new List<BillEntity>
        {
            new(1, 2, "payee name", 3, new DateOnly(), 4, "frequency name", 5, "category name", 6, "payer name"),
            new(62, 398, "TMCRaKglDO", 438, new DateOnly(), 797, "rshpoRRnmx", 713, "AHEqoPvGVx", 185, "liXwifejtC"),
            new(479, 259, "OAYEzcsXAu", 942, new DateOnly(), 436, "HOWUSFkJLd", 947, "NBOGcXgiXW", 91, "alGdvJZVPh"),
        };

        await billCache.SaveBills(authedUser1, bills);

        var result = await billCache.GetAllBills(authedUser2, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
