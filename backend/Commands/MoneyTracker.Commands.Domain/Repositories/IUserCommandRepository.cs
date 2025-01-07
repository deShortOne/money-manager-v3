using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IUserCommandRepository
{
    public Task AddUser(UserEntity userLogin);
    public Task<int> GetLastUserId();
    public Task<UserEntity?> GetUserByUsername(string username);
    public Task StoreTemporaryTokenToUser(UserAuthentication userAuthentication);
    public Task<UserAuthentication?> GetUserAuthFromToken(string token);
}
