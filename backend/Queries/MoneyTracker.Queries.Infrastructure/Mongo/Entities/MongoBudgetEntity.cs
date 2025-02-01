using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
internal class MongoBudgetEntity : MongoEntity
{
    public required AuthenticatedUser User { get; set; }
    public required List<BudgetGroupEntity> Budget { get; set; }
}
