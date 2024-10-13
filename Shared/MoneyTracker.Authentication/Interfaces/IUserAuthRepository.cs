using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IUserAuthRepository
{
    public Task<UserEntity?> GetUserByUsername(string username);
    public Task StoreTemporaryTokenToUser(AuthenticatedUser user, Guid token, DateTime expiration);
    public Task<TokenMapToUserEntity?> GetUserFromToken(Guid userGuid);
}
