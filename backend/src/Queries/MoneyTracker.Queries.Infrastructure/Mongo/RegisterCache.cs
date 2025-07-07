using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public class RegisterCache : IRegisterCache
{
    private IMongoCollection<MongoRegisterEntity> _registerCollection;

    public RegisterCache(MongoDatabase database)
    {
        _registerCollection = database.GetCollection<MongoRegisterEntity>("register");
    }

    public async Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var transactionsLisIterable = await _registerCollection.FindAsync(Builders<MongoRegisterEntity>.Filter.Eq(x => x.User, user), cancellationToken: cancellationToken);
        var registersLis = await transactionsLisIterable.ToListAsync(cancellationToken);
        if (registersLis.Count != 1)
        {
            return Error.NotFound("RegisterCache.GetAllTransactions", $"Found {registersLis.Count} registers for user {user}");
        }

        return registersLis[0].Transactions;
    }

    public Task<ResultT<ReceiptEntity>> GetReceiptProcessingInfo(string fileId, CancellationToken cancellationToken) => throw new NotImplementedException("Receipt processing data is never cached");
    public Task<ResultT<TemporaryTransaction>> GetTemporaryTransactionFromReceipt(string fileId, CancellationToken cancellationToken) => throw new NotImplementedException("Temporary transactions are never cached");

    public async Task<Result> SaveTransactions(AuthenticatedUser user, List<TransactionEntity> transactions,
        CancellationToken cancellationToken)
    {
        await _registerCollection.InsertOneAsync(new MongoRegisterEntity()
        {
            User = user,
            Transactions = transactions,
        }, cancellationToken: cancellationToken);

        return Result.Success();
    }
}
