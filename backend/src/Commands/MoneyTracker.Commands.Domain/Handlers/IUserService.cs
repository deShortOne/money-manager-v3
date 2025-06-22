using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IUserService
{
    Task<Result> AddNewUser(LoginWithUsernameAndPassword usernameAndPassword);
    Task<Result> LoginUser(LoginWithUsernameAndPassword usernameAndPassword);
    Task<ResultT<AuthenticatedUser>> GetUserFromToken(string token);
}
