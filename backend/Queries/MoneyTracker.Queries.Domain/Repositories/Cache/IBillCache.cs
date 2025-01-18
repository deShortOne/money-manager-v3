
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Domain.Repositories.Cache;
public interface IBillCache : IBillDatabase
{
    Task<Result> SaveBills(AuthenticatedUser user, List<BillEntity> bills);
}
