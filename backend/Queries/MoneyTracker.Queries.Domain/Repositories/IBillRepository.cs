using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IBillRepository
{
    public Task<List<BillEntity>> GetAllBills(AuthenticatedUser user);
}
