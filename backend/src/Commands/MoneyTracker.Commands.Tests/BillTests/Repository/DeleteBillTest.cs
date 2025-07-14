
namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class DeleteBillTest : BillRespositoryTestHelper
{
    [Fact]
    public async Task DeleteBillsFromSeed()
    {
        await _billRepo.DeleteBill(1, CancellationToken.None);
        await _billRepo.DeleteBill(2, CancellationToken.None);
        await _billRepo.DeleteBill(3, CancellationToken.None);

        Assert.Multiple(async () =>
        {
            Assert.Empty(await GetAllBillEntity());
        });
    }
}
