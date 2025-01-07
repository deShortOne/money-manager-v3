
using MoneyTracker.Authentication.Entities;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByUsername(string username);
    Task<string?> GetLastUserTokenForUser(UserEntity user);
    public Task<UserAuthentication?> GetUserAuthFromToken(string token);
}
