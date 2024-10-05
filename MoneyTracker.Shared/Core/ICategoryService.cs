using MoneyTracker.Shared.Models.ControllerToService.Category;
using MoneyTracker.Shared.Models.ServiceToController.Category;

namespace MoneyTracker.Shared.Core;
public interface ICategoryService
{
    Task AddCategory(NewCategoryRequestDTO categoryName);
    Task DeleteCategory(DeleteCategoryRequestDTO deleteCategory);
    Task EditCategory(EditCategoryRequestDTO editCategory);
    Task<List<CategoryResponseDTO>> GetAllCategories();
}
