
using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface IUserRepositoryService
{
    Task<UserEntity?> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task<string?> GetLastUserTokenForUser(UserEntity user, CancellationToken cancellationToken);
    Task<IUserAuthentication?> GetUserAuthFromToken(string token, CancellationToken cancellationToken);
    Task ResetUsersCache();
}
