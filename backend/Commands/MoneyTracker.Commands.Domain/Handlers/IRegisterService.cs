using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IRegisterService
{
    Task AddTransaction(string token, NewTransactionRequest newTransaction);
    Task DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction);
    Task EditTransaction(string token, EditTransactionRequest editTransaction);
}
