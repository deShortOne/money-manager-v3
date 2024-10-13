using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class AccountService : IAccountService
{
    private readonly IAccountRepository _dbService;

    public AccountService(IAccountRepository dbService)
    {
        _dbService = dbService;
    }
    public async Task<List<AccountResponse>> GetAccounts(AuthenticatedUser user)
    {
        var dtoFromDb = await _dbService.GetAccounts(user);
        List<AccountResponse> res = [];
        foreach (var account in dtoFromDb)
        {
            res.Add(new AccountResponse(account.Id, account.Name));
        }
        return res;
    }
}
