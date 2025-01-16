
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Commands.Tests.BudgetTests.Repository;
public sealed class DeleteBudgetTest : BudgetRespositoryTestHelper
{
    [Fact]
    public async void DeleteBudgetCategoriesFromSeed()
    {
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 1));
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 2));
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 3));
        await _budgetRepo.DeleteBudgetCategory(new DeleteBudgetCategoryEntity(1, 1, 4));

        Assert.Multiple(async () =>
        {
            Assert.Empty(await GetAllBudgetCategoryEntities());
        });
    }
}
