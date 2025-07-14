using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IUserService
{
    Task<Result> AddNewUser(LoginWithUsernameAndPassword usernameAndPassword, CancellationToken cancellationToken);
    Task<Result> LoginUser(LoginWithUsernameAndPassword usernameAndPassword, CancellationToken cancellationToken);
    Task<ResultT<AuthenticatedUser>> GetUserFromToken(string token, CancellationToken cancellationToken);
}
