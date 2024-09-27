using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;

namespace MoneyTracker.Shared.Core;
public interface IRegisterService
{
    Task<TransactionResponseDTO> AddTransaction(string token, NewTransactionRequestDTO newTransaction);
    Task<bool> DeleteTransaction(string token, DeleteTransactionRequestDTO deleteTransaction);
    Task<TransactionResponseDTO> EditTransaction(string token, EditTransactionRequestDTO editTransaction);
    Task<List<TransactionResponseDTO>> GetAllTransactions(string token);
}
