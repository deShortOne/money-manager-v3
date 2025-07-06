
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public class WhenGettingReceiptProcessingInfo : DatabaseOnlyTestHelper
{
    private string _filename;
    private ReceiptEntity _receiptState;
    private ResultT<ReceiptEntity> _result;

    public override async Task InitializeAsync()
    {
        _filename = "";
        _receiptState = new ReceiptEntity("", 1, _filename, "", ReceiptState.Processing);

        _mockRegisterDatabase
            .Setup(x => x.GetReceiptProcessingInfo(_filename, CancellationToken.None))
            .ReturnsAsync(_receiptState);

        _result = await _registerRepositoryService.GetReceiptProcessingInfo(_filename, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenDatabaseIsCalledOnce()
    {
        _mockRegisterDatabase.Verify(x => x.GetReceiptProcessingInfo(_filename, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheDataIsMappedCorrectly()
    {
        Assert.Equal(_receiptState, _result.Value);
    }
}
