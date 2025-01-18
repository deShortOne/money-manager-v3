using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IBudgetRepositoryService
{
    public Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user);
}
