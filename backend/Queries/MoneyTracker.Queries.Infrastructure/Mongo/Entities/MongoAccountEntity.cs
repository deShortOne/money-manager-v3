using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
internal sealed class MongoAccountEntity : MongoEntity
{
    public required AuthenticatedUser User { get; set; }
    public required List<AccountEntity> Accounts { get; set; }
}
