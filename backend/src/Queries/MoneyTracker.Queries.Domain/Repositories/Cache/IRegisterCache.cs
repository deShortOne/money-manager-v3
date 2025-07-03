
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Domain.Repositories.Cache;
public interface IRegisterCache : IRegisterDatabase
{
    Task<Result> SaveTransactions(AuthenticatedUser user, List<TransactionEntity> transactions,
        CancellationToken cancellationToken);
}
