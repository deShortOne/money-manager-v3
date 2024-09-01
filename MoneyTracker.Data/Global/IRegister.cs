
using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.Data.Global
{
    public interface IRegister
    {
        public Task<List<TransactionDTO>> GetAllTransactions();
        public Task<TransactionDTO> AddNewTransaction(NewTransactionDTO transaction);
        public Task<TransactionDTO> EditTransaction(EditTransactionDTO tramsaction);
        public Task<bool> DeleteTransaction(DeleteTransactionDTO transaction);
    }
}
