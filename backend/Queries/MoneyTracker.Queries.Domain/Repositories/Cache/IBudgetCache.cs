
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Domain.Repositories.Cache;
public interface IBudgetCache : IBudgetDatabase
{
    Task<Result> SaveBudget(AuthenticatedUser user, List<BudgetGroupEntity> budget);
}
