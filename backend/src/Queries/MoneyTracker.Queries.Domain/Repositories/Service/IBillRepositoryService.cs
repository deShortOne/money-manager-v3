using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IBillRepositoryService
{
    Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user, CancellationToken cancellationToken);
    Task ResetBillsCache(AuthenticatedUser user, CancellationToken cancellationToken);
}
