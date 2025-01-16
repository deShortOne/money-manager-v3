using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterCommandRepository _registerDb;
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;

    public RegisterService(IRegisterCommandRepository registerDb,
        IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserService userService,
        IAccountService accountService
        )
    {
        _registerDb = registerDb;
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userService = userService;
        _accountService = accountService;
    }

    public async Task<Result> AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;

        var payerAccount = await _accountDb.GetAccountById(newTransaction.PayerId);
        if (payerAccount == null) // to be logged differently
        {
            return Result.Failure(Error.Validation("RegisterService.AddTransaction", "Payer account not found"));
        }
        if (payerAccount.UserId != user.Id)
        {
            return Result.Failure(Error.Validation("RegisterService.AddTransaction", "Payer account not found"));
        }
        var newTransactionId = _idGenerator.NewInt(await _registerDb.GetLastTransactionId());

        var dtoToDb = new TransactionEntity(newTransactionId,
            newTransaction.PayeeId,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.CategoryId,
            newTransaction.PayerId);

        await _registerDb.AddTransaction(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, editTransaction.Id))
        {
            return Result.Failure(Error.Validation("RegisterService.EditTransaction", "Transaction not found"));
        }
        if (editTransaction.PayerId != null)
        {
            var payerAccount = await _accountDb.GetAccountById((int)editTransaction.PayerId);
            if (payerAccount == null)
            {
                return Result.Failure(Error.Validation("RegisterService.EditTransaction", "Payer account not found"));
            }
            if (payerAccount.UserId != user.Id)
                return Result.Failure(Error.Validation("RegisterService.EditTransaction", "Payer account not found"));
        }

        var dtoToDb = new EditTransactionEntity(editTransaction.Id, editTransaction.PayeeId, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.CategoryId, editTransaction.PayerId);

        await _registerDb.EditTransaction(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await DoesUserOwnTransaction(user, deleteTransaction.Id))
        {
            return Result.Failure(Error.Validation("RegisterService.DeleteTransaction", "Transaction not found"));
        }

        await _registerDb.DeleteTransaction(deleteTransaction.Id);

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
