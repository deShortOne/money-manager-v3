using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.ServiceToController.Account;

namespace MoneyTracker.Core;
public interface IAccountService
{
    Task<List<AccountResponseDTO>> GetAccounts(AuthenticatedUser user);
}
