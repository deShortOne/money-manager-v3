using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Contracts.Responses.Account;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IAccountService
{
    Task<List<AccountResponse>> GetAccounts(string token);
}
