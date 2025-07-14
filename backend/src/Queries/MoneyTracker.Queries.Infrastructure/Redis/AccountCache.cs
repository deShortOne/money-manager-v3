
using System.Text.Json;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using StackExchange.Redis;

namespace MoneyTracker.Queries.Infrastructure.Redis;
[Obsolete("This is to validate that this is abstracted properly")]
// and leaving MongoDatabase as a parameter to AccountCache is fine even if it's not easily mockable
public class AccountCache : IAccountCache
{
    private IDatabase _db;

    public AccountCache()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        _db = redis.GetDatabase();
    }

    public async Task<ResultT<List<AccountEntity>>> GetAccountsOwnedByUser(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        var fetchedUserJson = await _db.StringGetAsync($"accounts:{user.Id}");
        if (fetchedUserJson == RedisValue.Null)
            return Error.NotFound("AccountCache.GetAccounts", $"No accounts found for user: {user.Id}"); ;

        var listOfAccounts = JsonSerializer.Deserialize<List<AccountEntity>>(fetchedUserJson);
        if (listOfAccounts is null)
            return Error.Validation("", $"Cannot parse list of accounts for user {user.Id}");

        return listOfAccounts;
    }

    public async Task<Result> SaveAccounts(AuthenticatedUser user, List<AccountEntity> accounts, CancellationToken cancellationToken)
    {
        var listOfAccounts = JsonSerializer.Serialize(accounts);
        await _db.StringSetAsync($"accounts:{user.Id}", listOfAccounts);

        return Result.Success();
    }
}
