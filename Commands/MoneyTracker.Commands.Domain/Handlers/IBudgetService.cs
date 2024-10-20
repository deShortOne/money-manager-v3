
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBudgetService
{
    Task AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget);
    Task DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory);
    Task EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory);
}
