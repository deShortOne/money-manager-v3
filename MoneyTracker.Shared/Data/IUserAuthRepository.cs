using MoneyTracker.Shared.Auth;

namespace MoneyTracker.Shared.Data;

public interface IUserAuthRepository
{
    public Task<UserEntity?> GetUserByUsername(string username);
    public Task StoreTemporaryTokenToUser(AuthenticatedUser user, Guid token, DateTime expiration);
    public Task<TokenMapToUserDTO?> GetUserFromToken(Guid userGuid);
}
