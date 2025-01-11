using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IUserService
{
    Task AddNewUser(LoginWithUsernameAndPassword usernameAndPassword);
    Task LoginUser(LoginWithUsernameAndPassword usernameAndPassword);
    Task<ResultT<AuthenticatedUser>> GetUserFromToken(string token);
}
