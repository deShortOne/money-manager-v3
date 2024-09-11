using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.Services.Infrastructure;
public class RegisterDbService
{
    private readonly IRegisterDatabase _database;

    public RegisterDbService(IRegisterDatabase database)
    {
        _database = database;
    }

    public Task<List<TransactionDTO>> GetAllTransactions()
    {
        return _database.GetAllTransactions();
    }

    public Task<TransactionDTO> AddTransaction(NewTransactionDTO newTransaction)
    {
        return _database.AddNewTransaction(newTransaction);
    }

    public Task<TransactionDTO> EditTransaction(EditTransactionDTO editTransaction)
    {
        return _database.EditTransaction(editTransaction);
    }

    public Task<bool> DeleteTransaction(DeleteTransactionDTO deleteTransaction)
    {
        return _database.DeleteTransaction(deleteTransaction);
    }
}
