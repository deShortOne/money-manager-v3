
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService;
public class WhenRequestingReceiptProcessingInfo : CacheAsideTestHelper
{
    private readonly string _receiptId = "id a";
    private readonly ReceiptEntity _entity;
    private ResultT<ReceiptEntity> _result;

    public WhenRequestingReceiptProcessingInfo()
    {
        _entity = new ReceiptEntity(_receiptId, 52, "", "", Common.Values.ReceiptState.Finished);
    }

    public override async Task InitializeAsync()
    {
        _mockRegisterDatabase
            .Setup(x => x.GetReceiptProcessingInfo(_receiptId, CancellationToken.None))
            .ReturnsAsync(_entity);

        _result = await _registerRepositoryService.GetReceiptProcessingInfo(_receiptId, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheDatabaseIsOnlyCalledOnce()
    {
        _mockRegisterDatabase.Verify(x => x.GetReceiptProcessingInfo(_receiptId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheCacheIsNeverCalled()
    {
        _mockRegisterCache.VerifyNoOtherCalls();
    }
}
