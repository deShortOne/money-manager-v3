using MoneyTracker.Shared.User;

namespace MoneyTracker.Shared.Data;

public interface IUserAuthDatabase
{
    public Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser userToLogIn);
}
