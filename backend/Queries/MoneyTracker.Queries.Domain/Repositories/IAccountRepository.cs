using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IAccountRepository
{
    Task<List<AccountEntity>> GetAccounts(AuthenticatedUser user);
}
