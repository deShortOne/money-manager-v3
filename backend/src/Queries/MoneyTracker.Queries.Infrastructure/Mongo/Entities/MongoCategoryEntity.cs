using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Infrastructure.Mongo.Entities;
internal class MongoCategoryEntity : MongoEntity
{
    public required List<CategoryEntity> Categories { get; set; }
}
