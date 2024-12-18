using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Category;

namespace MoneyTracker.Commands.Application;
public class CategoryService : ICategoryService
{
    private readonly ICategoryCommandRepository _dbService;
    private readonly IIdGenerator _idGenerator;

    public CategoryService(ICategoryCommandRepository dbService, IIdGenerator idGenerator)
    {
        _dbService = dbService;
        _idGenerator = idGenerator;
    }

    public async Task AddCategory(NewCategoryRequest newCategory)
    {
        var newCategoryId = _idGenerator.NewInt(await _dbService.GetLastCategoryId());
        var dtoToDb = new CategoryEntity(newCategoryId, newCategory.Name);

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
