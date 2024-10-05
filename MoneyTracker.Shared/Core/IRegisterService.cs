using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;

namespace MoneyTracker.Shared.Core;
public interface IRegisterService
{
    Task AddTransaction(string token, NewTransactionRequestDTO newTransaction);
    Task DeleteTransaction(string token, DeleteTransactionRequestDTO deleteTransaction);
    Task EditTransaction(string token, EditTransactionRequestDTO editTransaction);
    Task<List<TransactionResponseDTO>> GetAllTransactions(string token);
}
