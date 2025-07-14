using MoneyTracker.Commands.Domain.Entities.Account;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IAccountCommandRepository
{
    Task<AccountEntity?> GetAccountById(int accountId, CancellationToken cancellationToken);
    Task<AccountEntity?> GetAccountByName(string accountName, CancellationToken cancellationToken);
    Task<AccountUserEntity?> GetAccountUserEntity(int accountId, int userId, CancellationToken cancellationToken);
    Task<AccountUserEntity?> GetAccountUserEntity(string accountUserName, int userId, CancellationToken cancellationToken);
    Task<AccountUserEntity?> GetAccountUserEntity(int accountUserId, CancellationToken cancellationToken);
    Task AddAccountToUser(AccountUserEntity newAccountUserEntity, CancellationToken cancellationToken);
    Task AddAccount(AccountEntity newAccount, CancellationToken cancellationToken);
    Task<int> GetLastAccountId(CancellationToken cancellationToken);
    Task<int> GetLastAccountUserId(CancellationToken cancellationToken);
}
