
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class CategoryRepository : ICategoryRepositoryService
{
    private readonly ICategoryDatabase _categoryDatabase;
    private readonly ICategoryCache _categoryCache;

    public CategoryRepository(
        ICategoryDatabase categoryDatabase,
        ICategoryCache categoryCache
        )
    {
        _categoryDatabase = categoryDatabase;
        _categoryCache = categoryCache;
    }

    public async Task<ResultT<List<CategoryEntity>>> GetAllCategories(CancellationToken cancellationToken)
    {
        var result = await _categoryCache.GetAllCategories(cancellationToken);
        if (result.HasError)
        {
            result = await _categoryDatabase.GetAllCategories(cancellationToken);
            await _categoryCache.SaveCategories(result.Value, cancellationToken);
        }

        return result;
    }

    public async Task ResetCategoriesCache(CancellationToken cancellationToken)
    {
        var result = await _categoryDatabase.GetAllCategories(cancellationToken);
        await _categoryCache.SaveCategories(result.Value, cancellationToken);
    }
}
