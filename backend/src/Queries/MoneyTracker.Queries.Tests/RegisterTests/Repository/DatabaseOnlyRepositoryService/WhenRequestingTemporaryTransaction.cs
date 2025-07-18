
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public class WhenRequestingTemporaryTransaction : DatabaseOnlyTestHelper
{
    private string _filename;
    private TemporaryTransaction _receiptState;
    private ResultT<TemporaryTransaction> _result;

    public override async Task InitializeAsync()
    {
        _filename = "da file name";
        _receiptState = new TemporaryTransaction(1, _filename, null, null, null, null, null);

        _mockRegisterDatabase
            .Setup(x => x.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None))
            .ReturnsAsync(_receiptState);

        _result = await _registerRepositoryService.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenDatabaseIsCalledOnce()
    {
        _mockRegisterDatabase.Verify(x => x.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheDataIsMappedCorrectly()
    {
        Assert.Equal(_receiptState, _result.Value);
    }
}
