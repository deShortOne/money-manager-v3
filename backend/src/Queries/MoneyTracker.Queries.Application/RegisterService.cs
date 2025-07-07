
using Microsoft.AspNetCore.Mvc.Filters;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Contracts.Responses.Receipt;
using MoneyTracker.Contracts.Responses.Transaction;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterRepositoryService _registerRepository;
    private readonly IUserRepositoryService _userRepository;

    public RegisterService(IRegisterRepositoryService registerDatabase,
        IUserRepositoryService userRepository)
    {
        this._registerRepository = registerDatabase;
        _userRepository = userRepository;
    }

    public async Task<ResultT<List<TransactionResponse>>> GetAllTransactions(string token,
        CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var transactionsResult = await _registerRepository.GetAllTransactions(user, cancellationToken);
        if (transactionsResult.HasError)
            return transactionsResult.Error!;

        var res = new List<TransactionResponse>();
        foreach (var transaction in transactionsResult.Value)
        {
            res.Add(new(transaction.Id,
                new(
                    transaction.PayeeId,
                    transaction.PayeeName
                ),
                transaction.Amount,
                transaction.DatePaid,
                new(
                    transaction.CategoryId,
                    transaction.CategoryName
                ),
                new(
                    transaction.PayerId,
                    transaction.PayerName
                )));
        }
        return res;
    }

    public async Task<ResultT<ReceiptResponse>> GetTransactionFromReceipt(string token, string filename, CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var receiptState = await _registerRepository.GetReceiptProcessingInfo(filename, cancellationToken);
        if (receiptState.HasError)
            return receiptState.Error!;
        if (receiptState.Value.State == ReceiptState.Processing)
            return new ReceiptResponse("Processing", null);
        if (receiptState.Value.State == ReceiptState.Pending)
        {
            var temporaryTransactionResult = await _registerRepository.GetTemporaryTransactionFromReceipt(filename, cancellationToken);
            if (temporaryTransactionResult.HasError)
                return Error.Failure("", $"Cannot find temporary transaction for {filename}");

            var temporaryTransactionResponse = new TemporaryTransactionResponse
            (
                temporaryTransactionResult.Value.Payee,
                temporaryTransactionResult.Value.Amount,
                temporaryTransactionResult.Value.DatePaid,
                temporaryTransactionResult.Value.Category,
                temporaryTransactionResult.Value.Payer
            );
            return new ReceiptResponse("Pending", temporaryTransactionResponse);
        }

        return new ReceiptResponse("invalid", null);
    }
}
