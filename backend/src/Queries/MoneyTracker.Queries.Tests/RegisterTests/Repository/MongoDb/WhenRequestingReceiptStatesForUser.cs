
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.MongoDb;
public class WhenRequestingReceiptStatesForUser : IClassFixture<MongoDbFixture>
{
    private RegisterCache _registerCache;

    public WhenRequestingReceiptStatesForUser(MongoDbFixture mongoDbFixture)
    {
        _registerCache = new RegisterCache(new MongoDatabase(mongoDbFixture.ConnectionString));
    }

    [Fact]
    public async Task ThenAnExceptionIsThrown()
    {
        var message = await Assert.ThrowsAsync<NotImplementedException>(async () => await _registerCache.GetReceiptStatesForUser(new AuthenticatedUser(1), CancellationToken.None));

        Assert.Equal("Receipt id and state is never cached", message.Message);
    }
}
