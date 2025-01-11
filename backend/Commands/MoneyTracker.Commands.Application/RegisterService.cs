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
    private readonly IRegisterCommandRepository _dbService;
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserService _userService;

    public RegisterService(IRegisterCommandRepository dbService,
        IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserService userService
        )
    {
        _dbService = dbService;
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userService = userService;
    }

    public async Task<Result> AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _accountDb.IsAccountOwnedByUser(user, newTransaction.PayerId))
        {
            return Result.Failure(Error.Validation("RegisterService.AddTransaction", "Payer account not found"));
        }
        var newTransactionId = _idGenerator.NewInt(await _dbService.GetLastTransactionId());

        var dtoToDb = new TransactionEntity(newTransactionId,
            newTransaction.PayeeId,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.CategoryId,
            newTransaction.PayerId);

        await _dbService.AddTransaction(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _dbService.IsTransactionOwnedByUser(user, editTransaction.Id))
        {
            return Result.Failure(Error.Validation("RegisterService.EditTransaction", "Transaction not found"));
        }
        if (editTransaction.PayerId != null && !await _accountDb.IsAccountOwnedByUser(user, (int)editTransaction.PayerId))
        {
            return Result.Failure(Error.Validation("RegisterService.EditTransaction", "Payer account not found"));
        }

        var dtoToDb = new EditTransactionEntity(editTransaction.Id, editTransaction.PayeeId, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.CategoryId, editTransaction.PayerId);

        await _dbService.EditTransaction(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        if (!await _dbService.IsTransactionOwnedByUser(user, deleteTransaction.Id))
        {
            return Result.Failure(Error.Validation("RegisterService.DeleteTransaction", "Transaction not found"));
        }

        await _dbService.DeleteTransaction(deleteTransaction.Id);

        return Result.Success();
    }
}
