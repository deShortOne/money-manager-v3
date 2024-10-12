using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ControllerToService.Category;
using MoneyTracker.Shared.Models.ServiceToController.Category;
using MoneyTracker.Shared.Models.ServiceToRepository.Category;

namespace MoneyTracker.Core;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _dbService;

    public CategoryService(ICategoryRepository dbService)
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

    public async Task AddCategory(Shared.Models.ControllerToService.Category.NewCategoryRequestDTO categoryName)
    {
        var dtoToDb = new Shared.Models.ServiceToRepository.Category.NewCategoryDTO(categoryName.Name);

        await _dbService.AddCategory(dtoToDb);
    }

    public async Task EditCategory(EditCategoryRequestDTO editCategory)
    {
        var dtoToDb = new EditCategoryDTO(editCategory.Id, editCategory.Name);

        await _dbService.EditCategory(dtoToDb);
    }

    public async Task DeleteCategory(DeleteCategoryRequestDTO deleteCategory)
    {
        var dtoToDb = new DeleteCategoryDTO(deleteCategory.Id);

        await _dbService.DeleteCategory(dtoToDb);
    }
}
