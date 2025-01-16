using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IRegisterCommandRepository
{
    public Task<TransactionEntity?> GetTransaction(int transactionId);
    public Task AddTransaction(TransactionEntity transaction);
    public Task EditTransaction(EditTransactionEntity tramsaction);
    public Task DeleteTransaction(int transactionId);
    public Task<int> GetLastTransactionId();
}
