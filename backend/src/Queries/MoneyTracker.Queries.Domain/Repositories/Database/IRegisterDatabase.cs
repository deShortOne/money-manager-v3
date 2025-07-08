using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Domain.Entities.Transaction;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface IRegisterDatabase
{
    Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken);
    Task<ResultT<ReceiptEntity>> GetReceiptProcessingInfo(string fileId, CancellationToken cancellationToken);
    Task<ResultT<TemporaryTransaction>> GetTemporaryTransactionFromReceipt(string fileId, CancellationToken cancellationToken);
    Task<List<ReceiptIdAndStateEntity>> GetReceiptStatesForUser(AuthenticatedUser user, List<int> designatedStates, CancellationToken cancellationToken);
}
