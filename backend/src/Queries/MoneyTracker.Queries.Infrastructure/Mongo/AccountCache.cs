using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public class AccountCache : IAccountCache
{
    private readonly IMongoCollection<MongoAccountEntity> _accountsCollection;

    public AccountCache(MongoDatabase database)
    {
        _accountsCollection = database.GetCollection<MongoAccountEntity>("account");
    }

    public async Task<ResultT<List<AccountEntity>>> GetAccountsOwnedByUser(AuthenticatedUser user)
    {
        var accountsLisIterable = await _accountsCollection.FindAsync(Builders<MongoAccountEntity>.Filter.Eq(x => x.User, user));
        var accountsLis = await accountsLisIterable.ToListAsync();
        if (accountsLis.Count != 1)
        {
            return Error.NotFound("AccountCache.GetAccounts", $"Found {accountsLis.Count} accounts for user {user}");
        }

        return accountsLis[0].Accounts;
    }

    public async Task<Result> SaveAccounts(AuthenticatedUser user, List<AccountEntity> accounts)
    {
        await _accountsCollection.InsertOneAsync(new MongoAccountEntity()
        {
            User = user,
            Accounts = accounts,
        });

        return Result.Success();
    }
}
