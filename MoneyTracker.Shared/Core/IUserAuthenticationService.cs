using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Core;
public interface IUserAuthenticationService
{
    Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser user);
    AuthenticatedUser DecodeToken(string authHeader);
    Task<string> GenerateToken(UnauthenticatedUser user);
}
