
using MongoDB.Bson;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
public abstract class MongoEntity
{
    public ObjectId Id { get; set; }
}
