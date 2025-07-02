
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class BudgetRepository : IBudgetRepositoryService
{
    private readonly IBudgetDatabase _budgetDatabase;
    private readonly IBudgetCache _budgetCache;

    public BudgetRepository(
        IBudgetDatabase budgetDatabase,
        IBudgetCache budgetCache
        )
    {
        _budgetDatabase = budgetDatabase;
        _budgetCache = budgetCache;
    }

    public async Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var result = await _budgetCache.GetBudget(user, cancellationToken);
        if (result.HasError)
        {
            result = await _budgetDatabase.GetBudget(user, cancellationToken);
            await _budgetCache.SaveBudget(user, result.Value);
        }

        return result;
    }

    public async Task ResetBudgetCache(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        var result = await _budgetDatabase.GetBudget(user, cancellationToken);
        await _budgetCache.SaveBudget(user, result.Value);
    }
}
