using System.Transactions;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

namespace MoneyTracker.Core;
public class RegisterService : IRegisterService
{
    private readonly IRegisterDatabase _dbService;

    public RegisterService(IRegisterDatabase dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<TransactionResponseDTO>> GetAllTransactions()
    {
        var dtoFromDb = await _dbService.GetAllTransactions();
        List<TransactionResponseDTO> res = [];
        foreach (var transaction in dtoFromDb)
        {
            res.Add(new(transaction.Id, transaction.Payee, transaction.Amount,
                transaction.DatePaid, transaction.Category));
        }
        return res;
    }

    public async Task<TransactionResponseDTO> AddTransaction(NewTransactionRequestDTO newTransaction)
    {
        var dtoToDb = new NewTransactionDTO(newTransaction.Payee, newTransaction.Amount,
            newTransaction.DatePaid, newTransaction.Category);
        var dtoFromDb = await _dbService.AddTransaction(dtoToDb);
        return new(dtoFromDb.Id, dtoFromDb.Payee, dtoFromDb.Amount, dtoFromDb.DatePaid, dtoFromDb.Category);
    }

    public async Task<TransactionResponseDTO> EditTransaction(EditTransactionRequestDTO editTransaction)
    {
        var dtoToDb = new EditTransactionDTO(editTransaction.Id, editTransaction.Payee, editTransaction.Amount,
            editTransaction.DatePaid, editTransaction.Category);
        var dtoFromDb = await _dbService.EditTransaction(dtoToDb);
        return new(dtoFromDb.Id, dtoFromDb.Payee, dtoFromDb.Amount, dtoFromDb.DatePaid, dtoFromDb.Category);
    }

    public Task<bool> DeleteTransaction(DeleteTransactionRequestDTO deleteTransaction)
    {
        var dtoToDb = new DeleteTransactionDTO(deleteTransaction.Id);
        return _dbService.DeleteTransaction(dtoToDb);
    }
}
