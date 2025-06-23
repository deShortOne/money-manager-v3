
using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IUserRepositoryService
{
    Task<UserEntity?> GetUserByUsername(string username);
    Task<string?> GetLastUserTokenForUser(UserEntity user);
    Task<IUserAuthentication?> GetUserAuthFromToken(string token);
    Task ResetUsersCache();
}
