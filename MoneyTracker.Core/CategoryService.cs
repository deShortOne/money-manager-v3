using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.Core;
public class CategoryService : ICategoryService
{
    private readonly ICategoryDatabase _dbService;

    public CategoryService(ICategoryDatabase dbService)
    {
        _dbService = dbService;
    }

    public Task<List<CategoryDTO>> GetAllCategories()
    {
        return _dbService.GetAllCategories();
    }

    public Task<CategoryDTO> AddCategory(NewCategoryDTO categoryName)
    {
        return _dbService.AddCategory(categoryName);
    }

    public Task<CategoryDTO> EditCategory(EditCategoryDTO editCategory)
    {
        return _dbService.EditCategory(editCategory);
    }

    public Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategory)
    {
        return _dbService.DeleteCategory(deleteCategory);
    }
}
