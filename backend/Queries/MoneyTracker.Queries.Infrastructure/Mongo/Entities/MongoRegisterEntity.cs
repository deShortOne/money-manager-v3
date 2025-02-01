using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
internal class MongoRegisterEntity : MongoEntity
{
    public required AuthenticatedUser User { get; set; }
    public required List<TransactionEntity> Transactions { get; set; }
}
