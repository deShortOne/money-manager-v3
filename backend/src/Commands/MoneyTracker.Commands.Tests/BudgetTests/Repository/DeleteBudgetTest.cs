
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Commands.Tests.BudgetTests.Repository;
public sealed class DeleteBudgetTest : BudgetRespositoryTestHelper
{
    [Fact]
    public async Task DeleteBudgetCategoriesFromSeed()
    {
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 1), CancellationToken.None);
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 2), CancellationToken.None);
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 3), CancellationToken.None);
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 4), CancellationToken.None);

        Assert.Multiple(async () =>
        {
            Assert.Empty(await GetAllBudgetCategoryEntities());
        });
    }
}
