
using MoneyTracker.Queries.Infrastructure.Mongo;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.MongoDb;
public class WhenRequestingReceiptProcessingInfo
{
    private RegisterCache _registerCache;

    public WhenRequestingReceiptProcessingInfo()
    {
        _registerCache = new RegisterCache(Mock.Of<MongoDatabase>());
    }

    //[Fact]
    public async Task ThenAnExceptionIsThrown()
    {
        var message = await Assert.ThrowsAsync<NotImplementedException>(async () => await _registerCache.GetReceiptProcessingInfo("", CancellationToken.None));

        Assert.Equal("Receipt processing data is never cached", message.Message);
    }
}
