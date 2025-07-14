

using MoneyTracker.Authentication.Entities;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service.GivenARequestToGetTransactionFromReceipt;
public class WhenTheUserIsInvalid : RegisterTestHelper, IAsyncLifetime
{
    private readonly string _token = "";
    public async Task InitializeAsync()
    {
        _mockUserRepository
            .Setup(x => x.GetUserAuthFromToken(_token, CancellationToken.None))
            .ReturnsAsync((IUserAuthentication?)null);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ThenTheExceptionWaasThrown()
    {
        var message = await Assert.ThrowsAsync<InvalidDataException>(async () => await _registerService.GetTransactionFromReceipt(_token, "", CancellationToken.None));
        Assert.Equal("Token not found", message.Message);
    }
}
