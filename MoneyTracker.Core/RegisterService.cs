using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.Core;
public class RegisterService : IRegisterService
{
    private readonly IRegisterDatabase _dbService;

    public RegisterService(IRegisterDatabase dbService)
    {
        _dbService = dbService;
    }

    public Task<List<TransactionDTO>> GetAllTransactions()
    {
        return _dbService.GetAllTransactions();
    }

    public Task<TransactionDTO> AddTransaction(NewTransactionDTO newTransaction)
    {
        return _dbService.AddTransaction(newTransaction);
    }

    public Task<TransactionDTO> EditTransaction(EditTransactionDTO editTransaction)
    {
        return _dbService.EditTransaction(editTransaction);
    }

    public Task<bool> DeleteTransaction(DeleteTransactionDTO deleteTransaction)
    {
        return _dbService.DeleteTransaction(deleteTransaction);
    }
}
