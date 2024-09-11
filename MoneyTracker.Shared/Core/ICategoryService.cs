using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.Shared.Core;
public interface ICategoryService
{
    Task<CategoryDTO> AddCategory(NewCategoryDTO categoryName);
    Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategory);
    Task<CategoryDTO> EditCategory(EditCategoryDTO editCategory);
    Task<List<CategoryDTO>> GetAllCategories();
}
