using MoneyTracker.Commands.Domain.Entities.Account;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IAccountCommandRepository
{
    Task<AccountEntity?> GetAccountById(int accountId);
}
