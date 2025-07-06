
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
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

    public async Task<Result> GetTransactionFromReceipt(string token, string filename, CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var receiptState = await _registerRepository.GetReceiptProcessingInfo(filename, cancellationToken);

        return Result.Success();
    }
}
