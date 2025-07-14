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

    public async Task AddCategory(NewCategoryRequest newCategory, CancellationToken cancellationToken)
    {
        var newCategoryId = _idGenerator.NewInt(await _dbService.GetLastCategoryId(cancellationToken));
        var dtoToDb = new CategoryEntity(newCategoryId, newCategory.Name);

        await _dbService.AddCategory(dtoToDb, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), cancellationToken);
    }

    public async Task EditCategory(EditCategoryRequest editCategory, CancellationToken cancellationToken)
    {
        var dtoToDb = new EditCategoryEntity(editCategory.Id, editCategory.Name);

        await _dbService.EditCategory(dtoToDb, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), cancellationToken);
    }

    public async Task DeleteCategory(DeleteCategoryRequest deleteCategory, CancellationToken cancellationToken)
    {
        await _dbService.DeleteCategory(deleteCategory.Id, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(-1), DataTypes.Category), cancellationToken);
    }

    public async Task<bool> DoesCategoryExist(int categoryId, CancellationToken cancellationToken)
    {
        var category = await _dbService.GetCategory(categoryId, cancellationToken);

        return category != null;
    }
}
