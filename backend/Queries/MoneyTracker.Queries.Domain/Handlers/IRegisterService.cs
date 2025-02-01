using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Transaction;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IRegisterService
{
    Task<ResultT<List<TransactionResponse>>> GetAllTransactions(string token);
}
