using MoneyTracker.Commands.Domain.Entities.Account;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IAccountCommandRepository
{
    Task<AccountEntity?> GetAccountById(int accountId);
    Task<AccountEntity?> GetAccountByName(string accountName);
    Task<AccountUserEntity?> GetAccountUserEntity(int accountId, int userId);
    Task<AccountUserEntity?> GetAccountUserEntity(int accountUserId);
    Task AddAccountToUser(AccountUserEntity newAccountUserEntity);
    Task AddAccount(AccountEntity newAccount);
    Task<int> GetLastAccountId();
    Task<int> GetLastAccountUserId();
}
