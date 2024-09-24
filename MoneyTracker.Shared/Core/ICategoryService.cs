using MoneyTracker.Shared.Models.ControllerToService.Category;
using MoneyTracker.Shared.Models.ServiceToController.Category;

namespace MoneyTracker.Shared.Core;
public interface ICategoryService
{
    Task<CategoryResponseDTO> AddCategory(NewCategoryRequestDTO categoryName);
    Task<bool> DeleteCategory(DeleteCategoryRequestDTO deleteCategory);
    Task<CategoryResponseDTO> EditCategory(EditCategoryRequestDTO editCategory);
    Task<List<CategoryResponseDTO>> GetAllCategories();
}
