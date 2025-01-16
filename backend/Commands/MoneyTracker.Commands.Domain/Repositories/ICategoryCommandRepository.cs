
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface ICategoryCommandRepository
{
    public Task<CategoryEntity?> GetCategory(int categoryId);
    public Task AddCategory(CategoryEntity categoryName);
    public Task EditCategory(EditCategoryEntity editCategoryDTO);
    public Task DeleteCategory(int categoryId);
    public Task<int> GetLastCategoryId();
}
