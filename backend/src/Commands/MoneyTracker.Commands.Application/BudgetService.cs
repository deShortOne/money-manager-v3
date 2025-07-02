using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Budget;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetCommandRepository _dbService;
    private readonly IUserCommandRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IMessageBusClient _messageBus;

    public BudgetService(
        IBudgetCommandRepository dbService,
        IUserCommandRepository userRepository,
        IUserService userService,
        IMessageBusClient messageBus
        )
    {
        _dbService = dbService;
        _userRepository = userRepository;
        _userService = userService;
        _messageBus = messageBus;
    }

    public async Task<Result> AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget,
        CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetUserFromToken(token, cancellationToken);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new BudgetCategoryEntity(user.Id, newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);

        await _dbService.AddBudgetCategory(dtoToDb, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Budget), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory,
        CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetUserFromToken(token, cancellationToken);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new EditBudgetCategoryEntity(user.Id, editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);

        await _dbService.EditBudgetCategory(dtoToDb, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Budget), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory,
        CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetUserFromToken(token, cancellationToken);
        if (userResult.HasError)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new DeleteBudgetCategoryEntity(user.Id, deleteBudgetCategory.BudgetGroupId, deleteBudgetCategory.BudgetCategoryId);

        await _dbService.DeleteBudgetCategory(dtoToDb, cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(user, DataTypes.Budget), cancellationToken);

        return Result.Success();
    }
}
