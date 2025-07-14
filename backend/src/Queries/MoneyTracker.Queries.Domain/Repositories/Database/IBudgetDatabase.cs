using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface IBudgetDatabase
{
    public Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user, CancellationToken cancellationToken);
}
