
using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Models.ServiceToRepository.Account;
public class GetAccountDTO(AuthenticatedUser user)
{
    public AuthenticatedUser User { get; } = user;
}
