using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Contracts.Responses.Transaction;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class RegisterService : IRegisterService
{
    private readonly IRegisterRepository _dbService;
    private readonly IUserAuthenticationService _userAuthService;

    public RegisterService(IRegisterRepository dbService,
        IUserAuthenticationService userAuthService)
    {
        _dbService = dbService;
        _userAuthService = userAuthService;
    }

    public async Task<List<TransactionResponse>> GetAllTransactions(string token)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoFromDb = await _dbService.GetAllTransactions(user);
        List<TransactionResponse> res = [];
        foreach (var transaction in dtoFromDb)
        {
            res.Add(new(transaction.Id, transaction.Payee, transaction.Amount,
                transaction.DatePaid, transaction.CategoryName, transaction.AccountName));
        }
        return res;
    }
}
