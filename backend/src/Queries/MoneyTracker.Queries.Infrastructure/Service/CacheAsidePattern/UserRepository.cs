
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class UserRepository : IUserRepositoryService
{
    private readonly IUserDatabase _userDatabase;

    public UserRepository(
        IUserDatabase userDatabase
        )
    {
        _userDatabase = userDatabase;
    }

    // TODOOOOO CACHEEE
    public Task<string?> GetLastUserTokenForUser(UserEntity user, CancellationToken cancellationToken)
        => _userDatabase.GetLastUserTokenForUser(user, cancellationToken);

    public Task<IUserAuthentication?> GetUserAuthFromToken(string token, CancellationToken cancellationToken)
        => _userDatabase.GetUserAuthFromToken(token, cancellationToken);

    public Task<UserEntity?> GetUserByUsername(string username, CancellationToken cancellationToken)
        => _userDatabase.GetUserByUsername(username, cancellationToken);

    public async Task ResetUsersCache()
    {
        await Task.CompletedTask;
    }
}
