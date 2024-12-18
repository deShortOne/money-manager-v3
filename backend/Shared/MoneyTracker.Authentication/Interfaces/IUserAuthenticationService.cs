using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Authentication.Interfaces;
public interface IUserAuthenticationService
{
    Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword user);
    Task<string> GenerateToken(LoginWithUsernameAndPassword user);
    Task<AuthenticatedUser> DecodeToken(string token);
}
