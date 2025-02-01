using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IAccountRepositoryService
{
    Task<ResultT<List<AccountEntity>>> GetAccounts(AuthenticatedUser user);
    Task ResetAccountsCache(AuthenticatedUser user);
}
