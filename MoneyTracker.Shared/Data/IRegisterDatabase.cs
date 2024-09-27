
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

namespace MoneyTracker.Shared.Data;

public interface IRegisterDatabase
{
    public Task<List<TransactionEntityDTO>> GetAllTransactions(AuthenticatedUser user);
    public Task<TransactionEntityDTO> AddTransaction(NewTransactionDTO transaction);
    public Task<TransactionEntityDTO> EditTransaction(EditTransactionDTO tramsaction);
    public Task<bool> DeleteTransaction(DeleteTransactionDTO transaction);
    public Task<bool> IsTransactionOwnedByUser(AuthenticatedUser user, int transactionId);
}
