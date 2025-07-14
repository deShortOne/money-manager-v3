using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Receipt;
using MoneyTracker.Contracts.Responses.Transaction;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IRegisterService
{
    Task<ResultT<List<TransactionResponse>>> GetAllTransactions(string token, CancellationToken cancellationToken);
    Task<ResultT<ReceiptResponse>> GetTransactionFromReceipt(string token, string filename, CancellationToken cancellationToken);
    Task<ResultT<List<ReceiptIdAndStateResponse>>> GetReceiptsAndStatesForGivenUser(string token, CancellationToken cancellationToken);
}
