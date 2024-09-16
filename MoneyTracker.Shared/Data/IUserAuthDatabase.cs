using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Data;

public interface IUserAuthDatabase
{
    public Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser userToLogIn);
}
