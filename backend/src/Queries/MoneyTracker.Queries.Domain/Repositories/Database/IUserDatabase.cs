
using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface IUserDatabase
{
    Task<UserEntity?> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task<string?> GetLastUserTokenForUser(UserEntity user, CancellationToken cancellationToken);
    public Task<IUserAuthentication?> GetUserAuthFromToken(string token, CancellationToken cancellationToken);
}
