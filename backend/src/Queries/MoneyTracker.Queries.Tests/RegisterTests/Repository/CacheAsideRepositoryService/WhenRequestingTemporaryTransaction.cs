
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService;
public class WhenRequestingTemporaryTransaction : CacheAsideTestHelper
{
    private readonly string _filename = "filename a";
    private readonly TemporaryTransaction _entity;
    private ResultT<TemporaryTransaction> _result;

    public WhenRequestingTemporaryTransaction()
    {
        _entity = new TemporaryTransaction(52, _filename, null, null, null, null, null);
    }

    public override async Task InitializeAsync()
    {
        _mockRegisterDatabase
            .Setup(x => x.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None))
            .ReturnsAsync(_entity);

        _result = await _registerRepositoryService.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None);
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
        _mockRegisterDatabase.Verify(x => x.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheCacheIsNeverCalled()
    {
        _mockRegisterCache.VerifyNoOtherCalls();
    }
}
