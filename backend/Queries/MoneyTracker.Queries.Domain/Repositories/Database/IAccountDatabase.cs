using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface IAccountDatabase
{
    Task<ResultT<List<AccountEntity>>> GetAccountsOwnedByUser(AuthenticatedUser user);
}
