
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBudgetService
{
    Task<Result> AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget,
        CancellationToken cancellationToken);
    Task<Result> DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory,
        CancellationToken cancellationToken);
    Task<Result> EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory,
        CancellationToken cancellationToken);
}
