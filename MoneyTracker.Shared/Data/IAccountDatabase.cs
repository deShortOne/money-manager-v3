using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Account;

namespace MoneyTracker.Data.Postgres;
public interface IAccountDatabase
{
    Task<List<AccountEntityDTO>> GetAccounts(AuthenticatedUser user);
    Task<bool> IsAccountOwnedByUser(AuthenticatedUser user, int accountId);
}
