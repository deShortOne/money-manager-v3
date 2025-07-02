using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Budget;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IBudgetService
{
    Task<ResultT<List<BudgetGroupResponse>>> GetBudget(string token, CancellationToken cancellationToken);
}
