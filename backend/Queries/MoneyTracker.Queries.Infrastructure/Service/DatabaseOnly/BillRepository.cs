
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
public class BillRepository : IBillRepositoryService
{
    private readonly IBillDatabase _billDatabase;

    public BillRepository(IBillDatabase billDatabase)
    {
        _billDatabase = billDatabase;
    }

    public Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user)
    {
        return _billDatabase.GetAllBills(user);
    }
}
