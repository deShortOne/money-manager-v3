using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IAccountRepository
{
    Task<ResultT<List<AccountEntity>>> GetAccounts(AuthenticatedUser user);
}
