
using MoneyTracker.Authentication.Entities;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByUsername(string username);
    Task<string> GetUserToken(UserEntity user);
}