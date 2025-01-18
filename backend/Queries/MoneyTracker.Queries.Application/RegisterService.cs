using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Transaction;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterDatabase _dbService;
    private readonly IUserDatabase _userRepository;

    public RegisterService(IRegisterDatabase dbService,
        IUserDatabase userRepository)
    {
        _dbService = dbService;
        _userRepository = userRepository;
    }

    public async Task<ResultT<List<TransactionResponse>>> GetAllTransactions(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var transactionsResult = await _dbService.GetAllTransactions(user);
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
