using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterCommandRepository _dbService;
    private readonly IAccountCommandRepository _accountDb;
    private readonly IIdGenerator _idGenerator;
    private readonly IUserCommandRepository _userRepository;

    public RegisterService(IRegisterCommandRepository dbService,
        IAccountCommandRepository accountDb,
        IIdGenerator idGenerator,
        IUserCommandRepository userRepository)
    {
        _dbService = dbService;
        _accountDb = accountDb;
        _idGenerator = idGenerator;
        _userRepository = userRepository;
    }

    public async Task AddTransaction(string token, NewTransactionRequest newTransaction)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
        if (!await _accountDb.IsAccountOwnedByUser(user, newTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }
        var newTransactionId = _idGenerator.NewInt(await _dbService.GetLastTransactionId());

        var dtoToDb = new TransactionEntity(newTransactionId,
            newTransaction.Payee,
            newTransaction.Amount,
            newTransaction.DatePaid,
            newTransaction.Category,
            newTransaction.AccountId);

        await _dbService.AddTransaction(dtoToDb);
    }

    public async Task EditTransaction(string token, EditTransactionRequest editTransaction)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
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
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
        if (!await _dbService.IsTransactionOwnedByUser(user, deleteTransaction.Id))
        {
            throw new InvalidDataException("Transaction not found");
        }

        await _dbService.DeleteTransaction(deleteTransaction.Id);
    }
}
