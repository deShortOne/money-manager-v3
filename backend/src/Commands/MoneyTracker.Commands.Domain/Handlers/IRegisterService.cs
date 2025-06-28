using Microsoft.AspNetCore.Http;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IRegisterService
{
    Task<Result> AddTransaction(string token, NewTransactionRequest newTransaction);
    Task<Result> DeleteTransaction(string token, DeleteTransactionRequest deleteTransaction);
    Task<Result> EditTransaction(string token, EditTransactionRequest editTransaction);
    Task<bool> DoesUserOwnTransaction(AuthenticatedUser user, int transactionId);
    Task<Result> CreateTransactionFromReceipt(string token, IFormFile createTransactionFromReceipt);
}
