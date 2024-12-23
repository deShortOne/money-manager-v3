using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IUserCommandRepository
{
    public Task AddUser(UserEntity userLogin);
    public Task<int> GetLastUserId();
    public Task<UserEntity?> GetUserByUsername(string username);
    public Task StoreTemporaryTokenToUser(UserEntity user, string token, DateTime expiration);
}
