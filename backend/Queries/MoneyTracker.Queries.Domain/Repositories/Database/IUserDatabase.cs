
using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface IUserDatabase
{
    Task<UserEntity?> GetUserByUsername(string username);
    Task<string?> GetLastUserTokenForUser(UserEntity user);
    public Task<IUserAuthentication?> GetUserAuthFromToken(string token);
}
