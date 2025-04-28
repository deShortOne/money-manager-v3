using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Application;
public class AccountService : IAccountService
{
    private readonly IAccountDatabase _dbService;
    private readonly IUserRepositoryService _userRepository;

    public AccountService(
        IAccountDatabase dbService,
        IUserRepositoryService userRepository)
    {
        _dbService = dbService;
        _userRepository = userRepository;
    }
    public async Task<ResultT<List<AccountResponse>>> GetAccounts(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoFromDbResult = await _dbService.GetAccountsOwnedByUser(user);
        if (!dtoFromDbResult.IsSuccess)
            return dtoFromDbResult.Error!;

        List<AccountResponse> res = [];
        foreach (var account in dtoFromDbResult.Value)
        {
            res.Add(new AccountResponse(account.Id, account.Name));
        }
        return res;
    }
}
