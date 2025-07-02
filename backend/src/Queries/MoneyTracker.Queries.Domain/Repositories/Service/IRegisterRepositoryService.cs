using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IRegisterRepositoryService
{
    Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken);
    Task ResetTransactionsCache(AuthenticatedUser user, CancellationToken cancellationToken);
}
