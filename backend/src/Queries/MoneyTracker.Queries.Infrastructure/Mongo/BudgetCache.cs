using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public class BudgetCache : IBudgetCache
{
    private readonly IMongoCollection<MongoBudgetEntity> _budgetCollection;

    public BudgetCache(MongoDatabase database)
    {
        _budgetCollection = database.GetCollection<MongoBudgetEntity>("budget");
    }

    public async Task<ResultT<List<BudgetGroupEntity>>> GetBudget(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var budgetLisIterable = await _budgetCollection.FindAsync(Builders<MongoBudgetEntity>.Filter.Eq(x => x.User, user), cancellationToken: cancellationToken);
        var budgetLis = await budgetLisIterable.ToListAsync(cancellationToken: cancellationToken);
        if (budgetLis.Count != 1)
        {
            return Error.NotFound("BudgetCache.GetBudget", $"Found {budgetLis.Count} budget for user {user}");
        }

        return budgetLis[0].Budget;
    }

    public async Task<Result> SaveBudget(AuthenticatedUser user, List<BudgetGroupEntity> budget)
    {
        await _budgetCollection.InsertOneAsync(new MongoBudgetEntity()
        {
            User = user,
            Budget = budget,
        });

        return Result.Success();
    }
}
