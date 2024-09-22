using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Data;

public interface IUserAuthDatabase
{
    public Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword userToLogIn);
    public Task<Guid> GenerateTempGuidForUser(AuthenticatedUser user, DateTime expiration);
    public Task<GuidMapToUserDTO> GetUserFromGuid(Guid userGuid);
}
