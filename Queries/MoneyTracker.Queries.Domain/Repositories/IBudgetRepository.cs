using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IBudgetRepository
{
    public Task<List<BudgetGroupEntity>> GetBudget(AuthenticatedUser user);
}
