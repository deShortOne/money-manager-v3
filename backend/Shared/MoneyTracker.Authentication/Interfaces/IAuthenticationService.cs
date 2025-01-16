
namespace MoneyTracker.Authentication.Interfaces;
public interface IAuthenticationService
{
    public string GenerateToken(UserIdentity user, DateTime expiration);
    public UserIdentity DecodeToken(string token);
}
