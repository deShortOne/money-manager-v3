using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public class BillCache : IBillCache
{
    private readonly IMongoCollection<MongoBillEntity> _billCollection;

    public BillCache(MongoDatabase database)
    {
        _billCollection = database.GetCollection<MongoBillEntity>("bill");
    }

    public async Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var billsLisIterable = await _billCollection.FindAsync(Builders<MongoBillEntity>.Filter.Eq(x => x.User, user));
        var billsLis = await billsLisIterable.ToListAsync();
        if (billsLis.Count != 1)
        {
            return Error.NotFound("BillCache.GetAllBills", $"Found {billsLis.Count} bills for user {user}");
        }

        return billsLis[0].Bills;
    }

    public async Task<Result> SaveBills(AuthenticatedUser user, List<BillEntity> bills)
    {
        await _billCollection.InsertOneAsync(new MongoBillEntity()
        {
            User = user,
            Bills = bills,
        });

        return Result.Success();
    }
}
