using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Transaction;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterCommandRepository _registerDb;
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly IMessageBusClient _messageBus;

    public RegisterService(IRegisterCommandRepository registerDb,
        IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserService userService,
        IAccountService accountService,
        ICategoryService categoryService,
        IMessageBusClient messageBus
        )
    {
        _registerDb = registerDb;
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userService = userService;
        _accountService = accountService;
        _categoryService = categoryService;
        _messageBus = messageBus;
    }

    public async Task<Result> AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;

        var payerAccount = await _accountDb.GetAccountUserEntity(newTransaction.PayerId);
        if (payerAccount == null) // to be logged differently
        {
            return Error.Validation("RegisterService.AddTransaction", "Payer account not found");
        }
        if (!payerAccount.UserOwnsAccount || payerAccount.UserId != user.Id)
        {
            return Error.Validation("RegisterService.AddTransaction", "Payer account not found");
        }

        var payeeAccount = await _accountDb.GetAccountUserEntity(newTransaction.PayeeId);
        if (payeeAccount == null) // to be logged differently
        {
            return Error.Validation("RegisterService.AddTransaction", "Payee account not found");
        }
        if (payeeAccount.UserId != user.Id)
        {
            return Error.Validation("RegisterService.AddTransaction", "Payee account not found");
        }

        if (!await _categoryService.DoesCategoryExist(newTransaction.CategoryId))
        {
            return Error.Validation("RegisterService.AddTransaction", "Category not found");
        }

        var newTransactionId = _idGenerator.NewInt(await _registerDb.GetLastTransactionId());

        var dtoToDb = new TransactionEntity(newTransactionId,
            newTransaction.PayeeId,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.CategoryId,
            newTransaction.PayerId);

        await _registerDb.AddTransaction(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, editTransaction.Id))
        {
            return Error.Validation("RegisterService.EditTransaction", "Transaction not found");
        }
        if (editTransaction.PayerId != null)
        {
            var payerAccount = await _accountDb.GetAccountUserEntity((int)editTransaction.PayerId, user.Id);
            if (payerAccount == null)
            {
                return Error.Validation("RegisterService.EditTransaction", "Payer account not found");
            }
            if (!payerAccount.UserOwnsAccount)
                return Error.Validation("RegisterService.EditTransaction", "Payer account not found");
        }

        var dtoToDb = new EditTransactionEntity(editTransaction.Id, editTransaction.PayeeId, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.CategoryId, editTransaction.PayerId);

        await _registerDb.EditTransaction(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, deleteTransaction.Id))
        {
            return Error.Validation("RegisterService.DeleteTransaction", "Transaction not found");
        }

        await _registerDb.DeleteTransaction(deleteTransaction.Id);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Register), CancellationToken.None);

        return Result.Success();
    }

    public async Task<bool> DoesUserOwnTransaction(AuthenticatedUser user, int transactionId)
    {
        var transaction = await _registerDb.GetTransaction(transactionId);
        if (transaction == null)
            return false;

        return await _accountService.DoesUserOwnAccount(user, transaction.PayerId);
    }
}
