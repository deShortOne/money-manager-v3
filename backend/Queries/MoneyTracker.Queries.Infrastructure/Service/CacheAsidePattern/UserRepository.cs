
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
    public Task<string?> GetLastUserTokenForUser(UserEntity user)
        => _userDatabase.GetLastUserTokenForUser(user);

    public Task<IUserAuthentication?> GetUserAuthFromToken(string token)
        => _userDatabase.GetUserAuthFromToken(token);

    public Task<UserEntity?> GetUserByUsername(string username)
        => _userDatabase.GetUserByUsername(username);

    public async Task ResetUsersCache()
    {
        await Task.CompletedTask;
    }
}
