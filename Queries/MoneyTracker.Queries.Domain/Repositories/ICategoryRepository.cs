using MoneyTracker.Queries.Domain.Entities.Category;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface ICategoryRepository
{
    public Task<List<CategoryEntity>> GetAllCategories();
}
