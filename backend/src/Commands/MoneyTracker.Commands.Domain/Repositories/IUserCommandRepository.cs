using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IUserCommandRepository
{
    public Task AddUser(UserEntity userLogin, CancellationToken cancellationToken);
    public Task<int> GetLastUserId(CancellationToken cancellationToken);
    public Task<UserEntity?> GetUserByUsername(string username, CancellationToken cancellationToken);
    public Task StoreTemporaryTokenToUser(UserAuthentication userAuthentication, CancellationToken cancellationToken);
    public Task<UserAuthentication?> GetUserAuthFromToken(string token, CancellationToken cancellationToken);
}
