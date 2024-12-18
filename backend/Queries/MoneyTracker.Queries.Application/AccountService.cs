using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class AccountService : IAccountService
{
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IAccountRepository _dbService;

    public AccountService(IUserAuthenticationService userAuthService,
        IAccountRepository dbService)
    {
        _userAuthService = userAuthService;
        _dbService = dbService;
    }
    public async Task<List<AccountResponse>> GetAccounts(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoFromDb = await _dbService.GetAccounts(user);
        List<AccountResponse> res = [];
        foreach (var account in dtoFromDb)
        {
            res.Add(new AccountResponse(account.Id, account.Name));
        }
        return res;
    }
}
