using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

namespace MoneyTracker.Core;
public class RegisterService : IRegisterService
{
    private readonly IRegisterDatabase _dbService;
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IAccountDatabase _accountDb;

    public RegisterService(IRegisterDatabase dbService,
        IUserAuthenticationService userAuthService,
        IAccountDatabase accountDb)
    {
        _dbService = dbService;
        _userAuthService = userAuthService;
        _accountDb = accountDb;
    }

    public async Task<List<TransactionResponseDTO>> GetAllTransactions(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoFromDb = await _dbService.GetAllTransactions(user);
        List<TransactionResponseDTO> res = [];
        foreach (var transaction in dtoFromDb)
        {
            res.Add(new(transaction.Id, transaction.Payee, transaction.Amount,
                transaction.DatePaid, transaction.Category, transaction.AccountName));
        }
        return res;
    }

    public async Task<TransactionResponseDTO> AddTransaction(string token, NewTransactionRequestDTO newTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _accountDb.IsAccountOwnedByUser(user, newTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }

        var dtoToDb = new NewTransactionDTO(newTransaction.Payee, newTransaction.Amount,
            newTransaction.DatePaid, newTransaction.Category, newTransaction.AccountId);
        var dtoFromDb = await _dbService.AddTransaction(dtoToDb);
        return new(dtoFromDb.Id, dtoFromDb.Payee, dtoFromDb.Amount, dtoFromDb.DatePaid, dtoFromDb.Category, dtoFromDb.AccountName);
    }

    public async Task<TransactionResponseDTO> EditTransaction(string token, EditTransactionRequestDTO editTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (editTransaction.AccountId != null && !await _accountDb.IsAccountOwnedByUser(user, (int)editTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }

        var dtoToDb = new EditTransactionDTO(editTransaction.Id, editTransaction.Payee, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.Category, editTransaction.AccountId);
        var dtoFromDb = await _dbService.EditTransaction(dtoToDb);
        return new(dtoFromDb.Id, dtoFromDb.Payee, dtoFromDb.Amount, dtoFromDb.DatePaid, dtoFromDb.Category, dtoFromDb.AccountName);
    }

    public Task<bool> DeleteTransaction(string token, DeleteTransactionRequestDTO deleteTransaction)
    {
        var dtoToDb = new DeleteTransactionDTO(deleteTransaction.Id);
        return _dbService.DeleteTransaction(dtoToDb);
    }
}
