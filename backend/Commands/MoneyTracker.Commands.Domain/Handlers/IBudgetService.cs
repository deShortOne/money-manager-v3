
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBudgetService
{
    Task<Result> AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget);
    Task<Result> DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory);
    Task<Result> EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory);
}
