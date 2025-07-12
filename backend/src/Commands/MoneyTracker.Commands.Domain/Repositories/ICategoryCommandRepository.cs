
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface ICategoryCommandRepository
{
    public Task<CategoryEntity?> GetCategory(int categoryId, CancellationToken cancellationToken);
    public Task<CategoryEntity?> GetCategory(string categoryName, CancellationToken cancellationToken);
    public Task AddCategory(CategoryEntity categoryName, CancellationToken cancellationToken);
    public Task EditCategory(EditCategoryEntity editCategoryDTO, CancellationToken cancellationToken);
    public Task DeleteCategory(int categoryId, CancellationToken cancellationToken);
    public Task<int> GetLastCategoryId(CancellationToken cancellationToken);
}
