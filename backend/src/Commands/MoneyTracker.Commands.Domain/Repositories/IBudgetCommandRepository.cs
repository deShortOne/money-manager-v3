using MoneyTracker.Commands.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IBudgetCommandRepository
{
    public Task AddBudgetCategory(BudgetCategoryEntity newBudget, CancellationToken cancellationToken);
    public Task EditBudgetCategory(EditBudgetCategoryEntity editBudgetCateogry, CancellationToken cancellationToken);
    public Task DeleteBudgetCategory(DeleteBudgetCategoryEntity deleteBudgetCategory,
        CancellationToken cancellationToken);
}
