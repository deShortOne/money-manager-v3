using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;

namespace MoneyTracker.Shared.Core;
public interface IRegisterService
{
    Task<TransactionResponseDTO> AddTransaction(NewTransactionRequestDTO newTransaction);
    Task<bool> DeleteTransaction(DeleteTransactionRequestDTO deleteTransaction);
    Task<TransactionResponseDTO> EditTransaction(EditTransactionRequestDTO editTransaction);
    Task<List<TransactionResponseDTO>> GetAllTransactions();
}
