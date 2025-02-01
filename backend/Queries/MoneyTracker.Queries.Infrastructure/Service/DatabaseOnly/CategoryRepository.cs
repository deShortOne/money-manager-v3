
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
public class CategoryRepository : ICategoryRepositoryService
{
    private readonly ICategoryDatabase _categoryDatabase;

    public CategoryRepository(ICategoryDatabase categoryDatabase)
    {
        _categoryDatabase = categoryDatabase;
    }

    public Task<ResultT<List<CategoryEntity>>> GetAllCategories()
    {
        return _categoryDatabase.GetAllCategories();
    }

    public Task ResetCategoriesCache() => throw new NotImplementedException();
}
