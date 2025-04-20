using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Account;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class AccountService : IAccountService
{
    private readonly IAccountCommandRepository _accountDb;
    private readonly IUserService _userService;
    private readonly IMessageBusClient _messageBus;

    public AccountService(IAccountCommandRepository accountDb,
        IUserService userService,
        IMessageBusClient messageBus
        )
    {
        _accountDb = accountDb;
        _userService = userService;
        _messageBus = messageBus;
    }

    public async Task<Result> AddAccount(string token, AddAccountToUserRequest newAccountRequest)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;

        var newAccountToUse = new AccountUserEntity(newAccountRequest.AccountId, user.Id, newAccountRequest.DoesUserOwnAccount);
        await _accountDb.AddAccountToUser(newAccountToUse);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Account), CancellationToken.None);

        return Result.Success();
    }

    public async Task<bool> DoesUserOwnAccount(AuthenticatedUser user, int accountId)
    {
        var account = await _accountDb.GetAccountUserEntity(accountId, user.Id);
        if (account == null)
            return false;

        return account.UserOwnsAccount;
    }
}
