
using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.Shared.Data
{
    public interface ICategoryDatabase
    {
        public Task<List<CategoryDTO>> GetAllCategories();
        public Task<CategoryDTO> AddCategory(NewCategoryDTO categoryName);
        public Task<CategoryDTO> EditCategory(EditCategoryDTO editCategoryDTO);
        public Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategoryDTO);
    }
}
