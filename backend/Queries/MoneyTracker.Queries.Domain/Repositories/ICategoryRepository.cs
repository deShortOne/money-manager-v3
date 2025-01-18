using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface ICategoryRepository
{
    public Task<ResultT<List<CategoryEntity>>> GetAllCategories();
}
