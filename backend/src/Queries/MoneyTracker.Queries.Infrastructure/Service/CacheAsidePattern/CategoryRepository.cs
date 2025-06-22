
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

    public async Task<ResultT<List<CategoryEntity>>> GetAllCategories()
    {
        var result = await _categoryCache.GetAllCategories();
        if (!result.IsSuccess)
        {
            result = await _categoryDatabase.GetAllCategories();
            await _categoryCache.SaveCategories(result.Value);
        }

        return result;
    }

    public async Task ResetCategoriesCache()
    {
        var result = await _categoryDatabase.GetAllCategories();
        await _categoryCache.SaveCategories(result.Value);
    }
}
