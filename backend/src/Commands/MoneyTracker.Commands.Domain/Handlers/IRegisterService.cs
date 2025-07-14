using Microsoft.AspNetCore.Http;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IRegisterService
{
    Task<ResultT<int>> AddTransaction(string token, NewTransactionRequest newTransaction, CancellationToken cancellationToken);
    Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction,
        CancellationToken cancellationToken);
    Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction,
        CancellationToken cancellationToken);
    Task<bool> DoesUserOwnTransaction(AuthenticatedUser user, int transactionId, CancellationToken cancellationToken);
    Task<ResultT<string>> CreateTransactionFromReceipt(string token, IFormFile createTransactionFromReceipt,
        CancellationToken cancellationToken);
    Task<Result> AddTransactionFromReceipt(string token, NewTransactionFromReceiptRequest newTransaction, CancellationToken cancellationToken);
}
