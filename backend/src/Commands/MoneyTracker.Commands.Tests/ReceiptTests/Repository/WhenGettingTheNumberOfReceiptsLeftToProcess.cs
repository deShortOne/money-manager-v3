
using MoneyTracker.Commands.Domain.Entities.Receipt;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public class WhenGettingTheNumberOfReceiptsLeftToProcess : ReceiptTestHelper
{
    private const string Id = "dga";
    private const int UserId = 1;
    private const string Name = "foobar";
    private const string Url = "url";
    private const int FinalTransactionId = 3;


    [Fact]
    public async Task TheTheCorrectNumberOfProcessingReceiptsAreReturned()
    {
        await SetupDatabase(CancellationToken.None);

        var result = await _receiptRepo.GetNumberOfReceiptsLeftToProcess(CancellationToken.None);

        Assert.Equal(3, result);
    }

    private async Task SetupDatabase(CancellationToken cancellationToken)
    {
        // Add 3 processing receipts
        for (var i = 0; i < 3; i++)
        {
            var receiptEntity = new ReceiptEntity(Id + i, UserId, Name + i, Url, 1, FinalTransactionId);
            await _receiptRepo.AddReceipt(receiptEntity, cancellationToken);
        }

        // Add 2 pending receipts
        for (var i = 0; i < 2; i++)
        {
            var receiptEntity = new ReceiptEntity(Id + i, UserId, Name + i, Url, 4, FinalTransactionId);
            await _receiptRepo.AddReceipt(receiptEntity, cancellationToken);
        }

        // Add 1 finished receipt
        for (var i = 0; i < 1; i++)
        {
            var receiptEntity = new ReceiptEntity(Id + i, UserId, Name + i, Url, 2, FinalTransactionId);
            await _receiptRepo.AddReceipt(receiptEntity, cancellationToken);
        }
    }
}
