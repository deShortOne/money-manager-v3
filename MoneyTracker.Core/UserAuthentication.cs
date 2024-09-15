
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.User;

namespace MoneyTracker.Core;
public class UserAuthenticationService
{
    private readonly IUserAuthDatabase _dbService;

    public UserAuthenticationService(IUserAuthDatabase dbService)
    {
        _dbService = dbService;
    }

    public Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser user)
    {
        return _dbService.AuthenticateUser(user);
    }
}
