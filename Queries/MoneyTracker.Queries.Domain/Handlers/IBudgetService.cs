using MoneyTracker.Contracts.Responses.Budget;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IBudgetService
{
    Task<List<BudgetGroupResponse>> GetBudget();
}
