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

    public async Task<ResultT<List<TransactionResponse>>> GetAllTransactions(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var transactionsResult = await _registerRepository.GetAllTransactions(user);
        if (!transactionsResult.IsSuccess)
            return transactionsResult.Error!;

        List<TransactionResponse> res = [];
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
}
