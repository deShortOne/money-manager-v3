using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Core;
public interface IUserAuthenticationService
{
    Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser user);
}
