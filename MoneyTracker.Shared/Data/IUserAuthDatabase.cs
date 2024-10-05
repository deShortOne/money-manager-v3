using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Data;

public interface IUserAuthDatabase
{
    public Task<UserEntity?> GetUserByUsername(string username);
    public Task<Guid> GenerateTempGuidForUser(AuthenticatedUser user, DateTime expiration);
    public Task<GuidMapToUserDTO> GetUserFromGuid(Guid userGuid);
}
