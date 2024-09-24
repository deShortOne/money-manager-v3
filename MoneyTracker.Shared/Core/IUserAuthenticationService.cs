using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Core;
public interface IUserAuthenticationService
{
    Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword user);
    Task<string> GenerateToken(LoginWithUsernameAndPassword user);
    Task<AuthenticatedUser> DecodeToken(string token);
}
