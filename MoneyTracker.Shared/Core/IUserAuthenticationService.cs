using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Core;
public interface IUserAuthenticationService
{
    Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword user);
    AuthenticatedUser DecodeToken(string authHeader);
    Task<string> GenerateToken(LoginWithUsernameAndPassword user);
}
