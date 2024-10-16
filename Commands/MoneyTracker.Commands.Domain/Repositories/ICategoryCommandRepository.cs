
using MoneyTracker.Commands.Domain.Entities.Category;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface ICategoryCommandRepository
{
    public Task AddCategory(CategoryEntity categoryName);
    public Task EditCategory(EditCategoryEntity editCategoryDTO);
    public Task DeleteCategory(int categoryId);
    Task<bool> DoesCategoryExist(int categoryId);
}
