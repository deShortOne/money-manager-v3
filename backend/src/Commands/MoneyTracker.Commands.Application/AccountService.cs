using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Account;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class AccountService : IAccountService
{
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserService _userService;
    private readonly IMessageBusClient _messageBus;

    public AccountService(IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserService userService,
        IMessageBusClient messageBus
        )
    {
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userService = userService;
        _messageBus = messageBus;
    }

    public async Task<Result> AddAccount(string token, AddAccountToUserRequest newAccountRequest,
        CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetUserFromToken(token, cancellationToken);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        var accountToAdd = await _accountDb.GetAccountByName(newAccountRequest.AccountName, cancellationToken);
        if (accountToAdd == null)
        {
            accountToAdd = new AccountEntity(_idGenerator.NewInt(await _accountDb.GetLastAccountId(cancellationToken)), newAccountRequest.AccountName);
            await _accountDb.AddAccount(accountToAdd, cancellationToken);
        }
        if (await _accountDb.GetAccountUserEntity(accountToAdd.Id, user.Id, cancellationToken) != null)
        {
            return Error.Validation("", "Account is already associated with user");
        }

        var newAccountUserId = _idGenerator.NewInt(await _accountDb.GetLastAccountUserId(cancellationToken));
        var newAccountToUse = new AccountUserEntity(
            newAccountUserId,
            accountToAdd.Id,
            user.Id,
            newAccountRequest.DoesUserOwnAccount
        );
        await _accountDb.AddAccountToUser(newAccountToUse, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Account), cancellationToken);

        return Result.Success();
    }

    public async Task<bool> DoesUserOwnAccount(AuthenticatedUser user, int accountId,
        CancellationToken cancellationToken)
    {
        var account = await _accountDb.GetAccountUserEntity(accountId, cancellationToken);
        if (account == null || account.UserId != user.Id)
            return false;

        return account.UserOwnsAccount;
    }
}
