using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IBillRepository
{
    public Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user);
}
