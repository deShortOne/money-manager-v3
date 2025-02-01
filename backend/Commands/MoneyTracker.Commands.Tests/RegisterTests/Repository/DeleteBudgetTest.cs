namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class DeleteBudgetTest : RegisterRespositoryTestHelper
{
    [Fact]
    public async Task DeleteBudgetCategoriesFromSeed()
    {
        await _registerRepo.DeleteTransaction(3);
        await _registerRepo.DeleteTransaction(8);
        await _registerRepo.DeleteTransaction(2);
        await _registerRepo.DeleteTransaction(4);

        var transactions = await GetAllTransactionEntities();
        Assert.Equal(6, transactions.Count);
    }
}
