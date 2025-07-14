
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Contracts.Responses.Receipt;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service.GivenARequestToGetTransactionFromReceipt;
public class WhenTheReceiptStateIsProcessing : RegisterTestHelper, IAsyncLifetime
{
    private readonly string _token = "da token";
    private readonly string _filename = "da file name";
    private ResultT<ReceiptResponse> _result;

    public async Task InitializeAsync()
    {
        var userAuthentication = new Mock<IUserAuthentication>();

        _mockUserRepository
            .Setup(x => x.GetUserAuthFromToken(_token, CancellationToken.None))
            .ReturnsAsync(userAuthentication.Object);

        _mockRegisterDatabase
            .Setup(x => x.GetReceiptProcessingInfo(_filename, CancellationToken.None))
            .ReturnsAsync(new ReceiptEntity("", -1, _filename, "", ReceiptState.Processing));

        _result = await _registerService.GetTransactionFromReceipt(_token, _filename, CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheStateOnTheResponseObjectIsMappedCorrectly()
    {
        Assert.Equal("Processing", _result.Value.ReceiptProcessingState);
    }

    [Fact]
    public void ThenTheTemporaryReceiptObjectIsNull()
    {
        Assert.Null(_result.Value.TemporaryTransaction);
    }
}
