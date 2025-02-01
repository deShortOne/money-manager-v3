using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IBudgetRepositoryService
{
    Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user);
    Task ResetBudgetCache(AuthenticatedUser user);
}
