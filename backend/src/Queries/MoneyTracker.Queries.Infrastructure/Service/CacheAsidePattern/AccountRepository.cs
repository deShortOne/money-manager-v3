
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class AccountRepository : IAccountRepositoryService
{
    private readonly IAccountDatabase _accountDatabase;
    private readonly IAccountCache _accountCache;

    public AccountRepository(
        IAccountDatabase accountDatabase,
        IAccountCache accountCache
        )
    {
        _accountDatabase = accountDatabase;
        _accountCache = accountCache;
    }
    public async Task<ResultT<List<AccountEntity>>> GetAccounts(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        ResultT<List<AccountEntity>> result = await _accountCache.GetAccountsOwnedByUser(user, cancellationToken);
        if (result.HasError)
        {
            result = await _accountDatabase.GetAccountsOwnedByUser(user, cancellationToken);
            await _accountCache.SaveAccounts(user, result.Value, cancellationToken);
        }

        return result;
    }

    public async Task ResetAccountsCache(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        ResultT<List<AccountEntity>> result = await _accountDatabase.GetAccountsOwnedByUser(user, cancellationToken);
        await _accountCache.SaveAccounts(user, result.Value, cancellationToken);
    }
}
