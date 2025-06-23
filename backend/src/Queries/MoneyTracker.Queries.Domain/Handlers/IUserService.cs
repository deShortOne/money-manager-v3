
using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IUserService
{
    Task<string> GetUserToken(LoginWithUsernameAndPassword userLogin);
    Task<bool> IsTokenValid(string token);
}
