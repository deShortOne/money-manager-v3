
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

namespace MoneyTracker.Shared.Data
{
    public interface IRegisterDatabase
    {
        public Task<List<TransactionEntityDTO>> GetAllTransactions(AuthenticatedUser user);
        public Task AddTransaction(NewTransactionDTO transaction);
        public Task EditTransaction(EditTransactionDTO tramsaction);
        public Task DeleteTransaction(DeleteTransactionDTO transaction);
        public Task<bool> IsTransactionOwnedByUser(AuthenticatedUser user, int transactionId);
    }
}
