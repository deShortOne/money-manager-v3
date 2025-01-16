
namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class DeleteBillTest : BillRespositoryTestHelper
{
    [Fact]
    public async void DeleteBillsFromSeed()
    {
        await _billRepo.DeleteBill(1);
        await _billRepo.DeleteBill(2);
        await _billRepo.DeleteBill(3);

        Assert.Multiple(async () =>
        {
            Assert.Empty(await GetAllBillEntity());
        });
    }
}
