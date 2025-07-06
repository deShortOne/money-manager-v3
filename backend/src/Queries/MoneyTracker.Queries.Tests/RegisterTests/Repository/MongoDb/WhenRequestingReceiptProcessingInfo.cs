
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.MongoDb;
public class WhenRequestingReceiptProcessingInfo : IClassFixture<MongoDbFixture>
{
    private RegisterCache _registerCache;

    public WhenRequestingReceiptProcessingInfo(MongoDbFixture mongoDbFixture)
    {
        _registerCache = new RegisterCache(new MongoDatabase(mongoDbFixture.ConnectionString));
    }

    [Fact]
    public async Task ThenAnExceptionIsThrown()
    {
        var message = await Assert.ThrowsAsync<NotImplementedException>(async () => await _registerCache.GetReceiptProcessingInfo("", CancellationToken.None));

        Assert.Equal("Receipt processing data is never cached", message.Message);
    }
}
