using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Domain.Repositories.Service;
public interface ICategoryRepositoryService
{
    Task<ResultT<List<CategoryEntity>>> GetAllCategories(CancellationToken cancellationToken);
    Task ResetCategoriesCache(CancellationToken cancellationToken);
}
