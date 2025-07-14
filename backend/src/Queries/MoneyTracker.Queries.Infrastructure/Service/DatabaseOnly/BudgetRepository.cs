
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
public class BudgetRepository : IBudgetRepositoryService
{
    private readonly IBudgetDatabase _budgetDatabase;

    public BudgetRepository(IBudgetDatabase budgetDatabase)
    {
        _budgetDatabase = budgetDatabase;
    }

    public Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        return _budgetDatabase.GetBudget(user, cancellationToken);
    }

    public Task ResetBudgetCache(AuthenticatedUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
}
