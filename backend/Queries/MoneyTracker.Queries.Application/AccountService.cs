using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class AccountService : IAccountService
{
    private readonly IAccountRepository _dbService;
    private readonly IUserRepository _userRepository;

    public AccountService(
        IAccountRepository dbService,
        IUserRepository userRepository)
    {
        _dbService = dbService;
        _userRepository = userRepository;
    }
    public async Task<List<AccountResponse>> GetAccounts(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.ThrowIfInvalid();

        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoFromDb = await _dbService.GetAccounts(user);
        List<AccountResponse> res = [];
        foreach (var account in dtoFromDb)
        {
            res.Add(new AccountResponse(account.Id, account.Name));
        }
        return res;
    }
}
