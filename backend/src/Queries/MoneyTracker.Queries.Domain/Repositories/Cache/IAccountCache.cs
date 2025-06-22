
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Domain.Repositories.Cache;
public interface IAccountCache : IAccountDatabase
{
    Task<Result> SaveAccounts(AuthenticatedUser user, List<AccountEntity> accounts);
}
