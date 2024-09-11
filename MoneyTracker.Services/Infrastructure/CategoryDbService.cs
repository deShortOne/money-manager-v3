using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.Services.Infrastructure;
public class CategoryDbService
{
    private readonly ICategoryDatabase _database;

    public CategoryDbService(ICategoryDatabase db)
    {
        _database = db;
    }

    public Task<List<CategoryDTO>> GetCategories()
    {
        return _database.GetAllCategories();
    }

    public Task<CategoryDTO> AddCategory(NewCategoryDTO categoryName)
    {
        return _database.AddCategory(categoryName);
    }

    public Task<CategoryDTO> EditCategory(EditCategoryDTO editCategory)
    {
        return _database.EditCategory(editCategory);
    }

    public Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategory)
    {
        return _database.DeleteCategory(deleteCategory);
    }
}
