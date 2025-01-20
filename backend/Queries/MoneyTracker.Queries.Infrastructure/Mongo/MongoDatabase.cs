
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public sealed class MongoDatabase
{
    private readonly IMongoDatabase _database;

    public MongoDatabase(string connectionString)
    {
        var client = new MongoClient(connectionString);

        _database = client.GetDatabase("moneytrackercache");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName) where T : MongoEntity
    {
        return _database.GetCollection<T>(collectionName);
    }
}
