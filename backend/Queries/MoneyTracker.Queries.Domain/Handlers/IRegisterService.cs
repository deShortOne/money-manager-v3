using MoneyTracker.Contracts.Responses.Transaction;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IRegisterService
{
    Task<List<TransactionResponse>> GetAllTransactions(string token);
}
