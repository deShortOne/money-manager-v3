using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.Contracts.Requests.Category;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class CategoryService : ICategoryService
{
    private readonly ICategoryCommandRepository _dbService;
    private readonly IIdGenerator _idGenerator;
    private readonly IMessageBusClient _messageBus;

    public CategoryService(ICategoryCommandRepository dbService,
        IIdGenerator idGenerator,
        IMessageBusClient messageBus
        )
    {
        _dbService = dbService;
        _idGenerator = idGenerator;
        _messageBus = messageBus;
    }

    public async Task AddCategory(NewCategoryRequest newCategory)
    {
        var newCategoryId = _idGenerator.NewInt(await _dbService.GetLastCategoryId());
        var dtoToDb = new CategoryEntity(newCategoryId, newCategory.Name);

        await _dbService.AddCategory(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), CancellationToken.None);
    }

    public async Task EditCategory(EditCategoryRequest editCategory)
    {
        var dtoToDb = new EditCategoryEntity(editCategory.Id, editCategory.Name);

        await _dbService.EditCategory(dtoToDb);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), CancellationToken.None);
    }

    public async Task DeleteCategory(DeleteCategoryRequest deleteCategory)
    {
        await _dbService.DeleteCategory(deleteCategory.Id);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), CancellationToken.None);
    }

    public async Task<bool> DoesCategoryExist(int categoryId)
    {
        var category = await _dbService.GetCategory(categoryId);

        return category != null;
    }
}
