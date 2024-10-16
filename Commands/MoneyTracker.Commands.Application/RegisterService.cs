using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterCommandRepository _dbService;
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IAccountCommandRepository _accountDb;

    public RegisterService(IRegisterCommandRepository dbService,
        IUserAuthenticationService userAuthService,
        IAccountCommandRepository accountDb)
    {
        _dbService = dbService;
        _userAuthService = userAuthService;
        _accountDb = accountDb;
    }

    public async Task AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _accountDb.IsAccountOwnedByUser(user, newTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }

        var dtoToDb = new TransactionEntity(1, // FIXME Set id
            newTransaction.Payee,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.Category,
            newTransaction.AccountId);

        await _dbService.AddTransaction(dtoToDb);
    }

    public async Task EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsTransactionOwnedByUser(user, editTransaction.Id))
        {
            throw new InvalidDataException("Transaction not found");
        }
        if (editTransaction.AccountId != null && !await _accountDb.IsAccountOwnedByUser(user, (int)editTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }

        var dtoToDb = new EditTransactionEntity(editTransaction.Id, editTransaction.Payee, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.Category, editTransaction.AccountId);

        await _dbService.EditTransaction(dtoToDb);
    }

    public async Task DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsTransactionOwnedByUser(user, deleteTransaction.Id))
        {
            throw new InvalidDataException("Transaction not found");
        }

        await _dbService.DeleteTransaction(deleteTransaction.Id);
    }
}
