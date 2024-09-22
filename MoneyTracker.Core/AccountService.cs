
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.ServiceToController.Account;

namespace MoneyTracker.Core;
public class AccountService : IAccountService
{
    private readonly IAccountDatabase _dbService;

    public AccountService(IAccountDatabase dbService)
    {
        _dbService = dbService;
    }
    public async Task<List<AccountResponseDTO>> GetAccounts(AuthenticatedUser user)
    {
        var dtoFromDb = await _dbService.GetAccounts(user);
        List<AccountResponseDTO> res = [];
        foreach (var account in dtoFromDb)
        {
            res.Add(new AccountResponseDTO(account.Id, account.Name));
        }
        return res;
    }
}
