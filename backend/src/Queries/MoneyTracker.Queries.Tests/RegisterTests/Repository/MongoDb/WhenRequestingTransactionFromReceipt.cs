
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.MongoDb;
public class WhenRequestingTransactionFromReceipt : IClassFixture<MongoDbFixture>
{
    private RegisterCache _registerCache;

    public WhenRequestingTransactionFromReceipt(MongoDbFixture mongoDbFixture)
    {
        _registerCache = new RegisterCache(new MongoDatabase(mongoDbFixture.ConnectionString));
    }

    [Fact]
    public async Task ThenAnExceptionIsThrown()
    {
        var message = await Assert.ThrowsAsync<NotImplementedException>(async () => await _registerCache.GetTemporaryTransactionFromReceipt("", CancellationToken.None));

        Assert.Equal("Temporary transactions are never cached", message.Message);
    }
}
