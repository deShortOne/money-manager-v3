using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface ICategoryRepositoryService
{
    public Task<ResultT<List<CategoryEntity>>> GetAllCategories();
}
