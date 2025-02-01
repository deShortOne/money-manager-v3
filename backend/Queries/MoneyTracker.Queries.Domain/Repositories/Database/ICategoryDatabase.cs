using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Domain.Repositories.Database;
public interface ICategoryDatabase
{
    public Task<ResultT<List<CategoryEntity>>> GetAllCategories();
}
