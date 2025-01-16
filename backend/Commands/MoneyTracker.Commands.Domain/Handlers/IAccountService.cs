using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IAccountService
{
    Task<bool> DoesUserOwnAccount(AuthenticatedUser user, int accountId);
}
