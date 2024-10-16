
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBudgetService
{
    Task AddBudgetCategory(NewBudgetCategoryRequest newBudget);
    Task DeleteBudgetCategory(DeleteBudgetCategoryRequest deleteBudgetCategory);
    Task EditBudgetCategory(EditBudgetCategoryRequest editBudgetCategory);
}
