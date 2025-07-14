
using MoneyTracker.Commands.Domain.Entities.Receipt;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public class WhenTryingToGetNonExistantReceipt : ReceiptTestHelper
{
    private Task<ReceiptEntity> _result;

    public async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _result = _receiptRepo.GetReceiptById("invalid id", CancellationToken.None);
    }

    [Fact]
    public void ThenNothingIsReturned()
    {
        Assert.Null(_result);
    }
}
