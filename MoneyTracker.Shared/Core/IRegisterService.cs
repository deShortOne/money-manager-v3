using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.Shared.Core;
public interface IRegisterService
{
    Task<TransactionDTO> AddTransaction(NewTransactionDTO newTransaction);
    Task<bool> DeleteTransaction(DeleteTransactionDTO deleteTransaction);
    Task<TransactionDTO> EditTransaction(EditTransactionDTO editTransaction);
    Task<List<TransactionDTO>> GetAllTransactions();
}
