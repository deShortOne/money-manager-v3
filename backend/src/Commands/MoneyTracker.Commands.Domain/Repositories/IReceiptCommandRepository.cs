
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IReceiptCommandRepository
{
    Task AddReceipt(ReceiptEntity receipt);
    Task UpdateReceipt(ReceiptEntity receipt);
    Task<ReceiptEntity?> GetReceiptById(string id);

    Task CreateTemporaryTransaction(int userId, TemporaryTransactionEntity temporaryTransactionEntity);
}
