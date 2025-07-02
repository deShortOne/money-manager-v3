
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class RegisterRepository : IRegisterRepositoryService
{
    private readonly IRegisterDatabase _registerDatabase;
    private readonly IRegisterCache _registerCache;

    public RegisterRepository(
        IRegisterDatabase registerDatabase,
        IRegisterCache registerCache
        )
    {
        _registerDatabase = registerDatabase;
        _registerCache = registerCache;
    }

    public async Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var result = await _registerCache.GetAllTransactions(user, cancellationToken);
        if (result.HasError)
        {
            result = await _registerDatabase.GetAllTransactions(user, cancellationToken);
            await _registerCache.SaveTransactions(user, result.Value);
        }

        return result;
    }

    public async Task ResetTransactionsCache(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        var result = await _registerDatabase.GetAllTransactions(user, cancellationToken);
        await _registerCache.SaveTransactions(user, result.Value);
    }
}
