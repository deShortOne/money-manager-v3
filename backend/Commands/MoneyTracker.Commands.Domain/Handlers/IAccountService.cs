using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Account;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IAccountService
{
    Task<bool> DoesUserOwnAccount(AuthenticatedUser user, int accountId);
    Task<Result> AddAccount(string token, AddAccountToUserRequest newAccountRequest);
}
