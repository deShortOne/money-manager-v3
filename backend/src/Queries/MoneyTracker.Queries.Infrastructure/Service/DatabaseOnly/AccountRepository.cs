
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
public class AccountRepository : IAccountRepositoryService
{
    private readonly IAccountDatabase _accountDatabase;

    public AccountRepository(IAccountDatabase accountDatabase)
    {
        _accountDatabase = accountDatabase;
    }
    public Task<ResultT<List<AccountEntity>>> GetAccounts(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        return _accountDatabase.GetAccountsOwnedByUser(user, cancellationToken);
    }

    public Task ResetAccountsCache(AuthenticatedUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
}
