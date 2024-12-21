using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IUserCommandRepository
{
    public Task AddUser(UserEntity userLogin);
    public Task<int> GetLastUserId();
}
