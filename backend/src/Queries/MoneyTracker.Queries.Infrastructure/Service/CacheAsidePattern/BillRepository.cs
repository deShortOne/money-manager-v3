
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class BillRepository : IBillRepositoryService
{
    private readonly IBillDatabase _billDatabase;
    private readonly IBillCache _billCache;

    public BillRepository(
        IBillDatabase billDatabase,
        IBillCache billCache
        )
    {
        _billDatabase = billDatabase;
        _billCache = billCache;
    }

    public async Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var result = await _billCache.GetAllBills(user, cancellationToken);
        if (result.HasError)
        {
            result = await _billDatabase.GetAllBills(user, cancellationToken);
            await _billCache.SaveBills(user, result.Value, cancellationToken);
        }

        return result;
    }

    public async Task ResetBillsCache(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        var result = await _billDatabase.GetAllBills(user, cancellationToken);
        await _billCache.SaveBills(user, result.Value, cancellationToken);
    }
}
