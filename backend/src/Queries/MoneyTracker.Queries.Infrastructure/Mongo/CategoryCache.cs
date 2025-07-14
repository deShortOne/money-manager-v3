using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Infrastructure.Mongo.Entities;
using MongoDB.Driver;

namespace MoneyTracker.Queries.Infrastructure.Mongo;
public class CategoryCache : ICategoryCache
{
    private IMongoCollection<MongoCategoryEntity> _categoriesCollection;

    public CategoryCache(MongoDatabase database)
    {
        _categoriesCollection = database.GetCollection<MongoCategoryEntity>("category");
    }

    public async Task<ResultT<List<CategoryEntity>>> GetAllCategories(CancellationToken cancellationToken)
    {
        var categoriessLisIterable = await _categoriesCollection.FindAsync(_ => true, cancellationToken: cancellationToken);
        var categoriessLis = await categoriessLisIterable.ToListAsync(cancellationToken);
        if (categoriessLis.Count != 1)
        {
            return Error.NotFound("CategoryCache.GetAllCategories", $"Found {categoriessLis.Count} category");
        }

        return categoriessLis[0].Categories;
    }

    public async Task<Result> SaveCategories(List<CategoryEntity> categories, CancellationToken cancellationToken)
    {
        await _categoriesCollection.InsertOneAsync(new MongoCategoryEntity()
        {
            Categories = categories,
        }, cancellationToken: cancellationToken);

        return Result.Success();
    }
}
