
using MoneyTracker.Shared.Models.RepositoryToService.Category;
using MoneyTracker.Shared.Models.ServiceToRepository.Category;

namespace MoneyTracker.Shared.Data;

public interface ICategoryDatabase
{
    public Task<List<CategoryEntityDTO>> GetAllCategories();
    public Task<CategoryEntityDTO> AddCategory(NewCategoryDTO categoryName);
    public Task<CategoryEntityDTO> EditCategory(EditCategoryDTO editCategoryDTO);
    public Task<bool> DeleteCategory(DeleteCategoryDTO deleteCategoryDTO);
}
