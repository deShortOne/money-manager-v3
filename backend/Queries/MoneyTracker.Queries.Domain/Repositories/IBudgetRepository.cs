using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IBudgetRepository
{
    public Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user);
}
