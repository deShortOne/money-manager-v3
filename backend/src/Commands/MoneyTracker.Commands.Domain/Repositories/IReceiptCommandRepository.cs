
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IReceiptCommandRepository
{
    Task AddReceipt(ReceiptEntity receipt, CancellationToken cancellationToken);
    Task UpdateReceipt(ReceiptEntity receipt, CancellationToken cancellationToken);
    Task<ReceiptEntity?> GetReceiptById(string id, CancellationToken cancellationToken);
    Task<int> GetNumberOfReceiptsLeftToProcess(CancellationToken cancellationToken);

    Task CreateTemporaryTransaction(TemporaryTransactionEntity temporaryTransactionEntity,
        CancellationToken cancellationToken);
}
