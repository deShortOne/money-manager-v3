using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Contracts.Requests.Category;

namespace MoneyTracker.Commands.Application;
public class CategoryService : ICategoryService
{
    private readonly ICategoryCommandRepository _dbService;

    public CategoryService(ICategoryCommandRepository dbService)
    {
        _dbService = dbService;
    }

    public async Task AddCategory(NewCategoryRequest newCategory)
    {
        var dtoToDb = new CategoryEntity(1, newCategory.Name); // FIXME generate id

        await _dbService.AddCategory(dtoToDb);
    }

    public async Task EditCategory(EditCategoryRequest editCategory)
    {
        var dtoToDb = new EditCategoryEntity(editCategory.Id, editCategory.Name);

        await _dbService.EditCategory(dtoToDb);
    }

    public async Task DeleteCategory(DeleteCategoryRequest deleteCategory)
    {
        await _dbService.DeleteCategory(deleteCategory.Id);
    }
}
