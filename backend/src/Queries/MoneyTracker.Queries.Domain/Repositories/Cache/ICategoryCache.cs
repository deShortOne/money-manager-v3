
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Domain.Repositories.Database;

namespace MoneyTracker.Queries.Domain.Repositories.Cache;
public interface ICategoryCache : ICategoryDatabase
{
    Task<Result> SaveCategories(List<CategoryEntity> categories, CancellationToken cancellationToken);
}
