using MoneyTracker.Commands.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IBudgetCommandRepository
{
    public Task AddBudgetCategory(BudgetCategoryEntity newBudget);
    public Task EditBudgetCategory(EditBudgetCategoryEntity editBudgetCateogry);
    public Task DeleteBudgetCategory(DeleteBudgetCategoryEntity deleteBudgetCategory);
}
