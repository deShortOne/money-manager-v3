using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Contracts.Responses.Transaction;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterRepository _dbService;
    private readonly IUserRepository _userRepository;

    public RegisterService(IRegisterRepository dbService,
        IUserRepository userRepository)
    {
        _dbService = dbService;
        _userRepository = userRepository;
    }

    public async Task<List<TransactionResponse>> GetAllTransactions(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoFromDb = await _dbService.GetAllTransactions(user);
        List<TransactionResponse> res = [];
        foreach (var transaction in dtoFromDb)
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
