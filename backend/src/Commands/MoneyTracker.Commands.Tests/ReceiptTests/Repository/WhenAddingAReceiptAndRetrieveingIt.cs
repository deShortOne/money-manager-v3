
using MoneyTracker.Commands.Domain.Entities.Receipt;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public class WhenAddingAReceiptAndRetrieveingIt : ReceiptTestHelper
{
    private const string Id = "dga";
    private const int UserId = 1;
    private const string Name = "foobar";
    private const string Url = "url";
    private const int State = 4;
    private const int FinalTransactionId = 3;

    [Fact]
    public async Task ThenTheEntityIsReturned()
    {
        var receiptEntity = new ReceiptEntity(Id, UserId, Name, Url, State, FinalTransactionId);

        await _receiptRepo.AddReceipt(receiptEntity, CancellationToken.None);

        var result = await _receiptRepo.GetReceiptById(Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Multiple(() =>
        {
            Assert.Equal(Id, result.Id);
            Assert.Equal(UserId, result.UserId);
            Assert.Equal(Name, result.Name);
            Assert.Equal(Url, result.Url);
            Assert.Equal(State, result.State);
            Assert.Equal(FinalTransactionId, result.FinalTransactionId);
        });
    }
}
