using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IRegisterCommandRepository
{
    public Task<TransactionEntity?> GetTransaction(int transactionId, CancellationToken cancellationToken);
    public Task AddTransaction(TransactionEntity transaction, CancellationToken cancellationToken);
    public Task EditTransaction(EditTransactionEntity tramsaction, CancellationToken cancellationToken);
    public Task DeleteTransaction(int transactionId, CancellationToken cancellationToken);
    public Task<int> GetLastTransactionId(CancellationToken cancellationToken);
}
