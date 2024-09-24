using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ControllerToService.Category;
using MoneyTracker.Shared.Models.ServiceToController.Category;
using MoneyTracker.Shared.Models.ServiceToRepository.Category;

namespace MoneyTracker.Core;
public class CategoryService : ICategoryService
{
    private readonly ICategoryDatabase _dbService;

    public CategoryService(ICategoryDatabase dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<CategoryResponseDTO>> GetAllCategories()
    {
        var dtoLisFromDb = await _dbService.GetAllCategories();
        List<CategoryResponseDTO> res = [];
        foreach (var category in dtoLisFromDb)
        {
            res.Add(new(category.Id, category.Name));
        }
        return res;
    }

    public async Task<CategoryResponseDTO> AddCategory(Shared.Models.ControllerToService.Category.NewCategoryRequestDTO categoryName)
    {
        var dtoToDb = new Shared.Models.ServiceToRepository.Category.NewCategoryDTO(categoryName.Name);
        var dtoFromDb = await _dbService.AddCategory(dtoToDb);
        return new CategoryResponseDTO(dtoFromDb.Id, dtoFromDb.Name);
    }

    public async Task<CategoryResponseDTO> EditCategory(EditCategoryRequestDTO editCategory)
    {
        var dtoToDb = new EditCategoryDTO(editCategory.Id, editCategory.Name);
        var dtoFromDb = await _dbService.EditCategory(dtoToDb);
        return new CategoryResponseDTO(dtoFromDb.Id, dtoFromDb.Name);
    }

    public Task<bool> DeleteCategory(DeleteCategoryRequestDTO deleteCategory)
    {
        var dtoToDb = new DeleteCategoryDTO(deleteCategory.Id);
        return _dbService.DeleteCategory(dtoToDb);
    }
}
