using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
internal class MongoBillEntity : MongoEntity
{
    public required AuthenticatedUser User { get; set; }
    public required List<BillEntity> Bills { get; set; }
}
