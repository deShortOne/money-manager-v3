
using MoneyTracker.Commands.Domain.Entities.Receipt;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public class WhenUpdatingAReceipt : ReceiptTestHelper
{
    private const string Id = "dga";
    private const int UserId = 1;
    private const string Name = "foobar";
    private const string Url = "url";
    private const int State = 4;

    private const int FinalState = 3;
    private const int FinalTransactionId = 3;


    [Fact]
    public async Task ThenTheEntityIsReturned()
    {
        var receiptEntity = new ReceiptEntity(Id, UserId, Name, Url, State, null);

        await _receiptRepo.AddReceipt(receiptEntity, CancellationToken.None);

        receiptEntity.UpdateState(FinalState);
        receiptEntity.SetFinalTransactionId(FinalTransactionId);
        await _receiptRepo.UpdateReceipt(receiptEntity, CancellationToken.None);

        var result = await _receiptRepo.GetReceiptById(Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Multiple(() =>
        {
            Assert.Equal(Id, result.Id);
            Assert.Equal(UserId, result.UserId);
            Assert.Equal(Name, result.Name);
            Assert.Equal(Url, result.Url);
            Assert.Equal(FinalState, result.State);
            Assert.Equal(FinalTransactionId, result.FinalTransactionId);
        });
    }
}

