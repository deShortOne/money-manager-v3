
using MoneyTracker.Authentication.DTOs;

public interface IUserService
{
    Task<string> GetUserToken(LoginWithUsernameAndPassword userLogin);
}