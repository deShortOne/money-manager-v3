using MoneyTracker.Shared.Models.RepositoryToService.Account;
using MoneyTracker.Shared.User;

namespace MoneyTracker.Data.Postgres;
public interface IAccountDatabase
{
    Task<List<AccountEntityDTO>> GetAccounts(AuthenticatedUser user);
}
