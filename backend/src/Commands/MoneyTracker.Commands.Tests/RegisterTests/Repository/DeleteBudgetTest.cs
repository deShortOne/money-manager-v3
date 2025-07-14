namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class DeleteBudgetTest : RegisterRespositoryTestHelper
{
    [Fact]
    public async Task DeleteBudgetCategoriesFromSeed()
    {
        await _registerRepo.DeleteTransaction(3, CancellationToken.None);
        await _registerRepo.DeleteTransaction(8, CancellationToken.None);
        await _registerRepo.DeleteTransaction(2, CancellationToken.None);
        await _registerRepo.DeleteTransaction(4, CancellationToken.None);

        var transactions = await GetAllTransactionEntities();
        Assert.Equal(6, transactions.Count);
    }
}
