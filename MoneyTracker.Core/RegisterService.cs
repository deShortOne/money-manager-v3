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

    public async Task AddTransaction(string token, NewTransactionRequestDTO newTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _accountDb.IsAccountOwnedByUser(user, newTransaction.AccountId))
        {
            throw new InvalidDataException("Account not found");
        }

        var dtoToDb = new NewTransactionDTO(newTransaction.Payee, newTransaction.Amount,
            newTransaction.DatePaid, newTransaction.Category, newTransaction.AccountId);

        await _dbService.AddTransaction(dtoToDb);
    }

    public async Task EditTransaction(string token, EditTransactionRequestDTO editTransaction)
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

        var dtoToDb = new EditTransactionDTO(editTransaction.Id, editTransaction.Payee, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.Category, editTransaction.AccountId);

        await _dbService.EditTransaction(dtoToDb);
    }

    public async Task DeleteTransaction(string token, DeleteTransactionRequestDTO deleteTransaction)
    {
        var user = await _userAuthService.DecodeToken(token);
        if (!await _dbService.IsTransactionOwnedByUser(user, deleteTransaction.Id))
        {
            throw new InvalidDataException("Transaction not found");
        }

        var dtoToDb = new DeleteTransactionDTO(deleteTransaction.Id);
        await _dbService.DeleteTransaction(dtoToDb);
    }
}
