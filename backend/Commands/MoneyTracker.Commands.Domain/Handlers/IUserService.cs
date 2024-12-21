using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IUserService
{
    Task AddNewUser(LoginWithUsernameAndPassword usernameAndPassword);
}
