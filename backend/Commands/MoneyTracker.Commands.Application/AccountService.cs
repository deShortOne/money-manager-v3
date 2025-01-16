
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
public class AccountService : IAccountService
{
    private readonly IAccountCommandRepository _accountDb;

    public AccountService(IAccountCommandRepository accountDb)
    {
        _accountDb = accountDb;
    }

    public async Task<bool> DoesUserOwnAccount(AuthenticatedUser user, int accountId)
    {
        var account = await _accountDb.GetAccountById(accountId);
        if (account == null)
            return false;

        return account.UserId == user.Id;
    }
}
