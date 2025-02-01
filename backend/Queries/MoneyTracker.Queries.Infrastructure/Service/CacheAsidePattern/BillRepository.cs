
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

    public async Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user)
    {
        var result = await _billCache.GetAllBills(user);
        if (!result.IsSuccess)
        {
            result = await _billDatabase.GetAllBills(user);
            await _billCache.SaveBills(user, result.Value);
        }

        return result;
    }

    public async Task ResetBillsCache(AuthenticatedUser user)
    {
        var result = await _billDatabase.GetAllBills(user);
        await _billCache.SaveBills(user, result.Value);
    }
}
