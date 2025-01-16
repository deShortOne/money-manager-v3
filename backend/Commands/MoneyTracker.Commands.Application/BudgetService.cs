using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetCommandRepository _dbService;
    private readonly IUserCommandRepository _userRepository;
    private readonly IUserService _userService;

    public BudgetService(
        IBudgetCommandRepository dbService,
        IUserCommandRepository userRepository,
        IUserService userService
        )
    {
        _dbService = dbService;
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<Result> AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new BudgetCategoryEntity(user.Id, newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);

        await _dbService.AddBudgetCategory(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new EditBudgetCategoryEntity(user.Id, editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);

        await _dbService.EditBudgetCategory(dtoToDb);

        return Result.Success();
    }

    public async Task<Result> DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory)
    {
        var userResult = await _userService.GetUserFromToken(token);
        if (!userResult.IsSuccess)
            return userResult;

        var user = userResult.Value;
        var dtoToDb = new DeleteBudgetCategoryEntity(user.Id, deleteBudgetCategory.BudgetGroupId, deleteBudgetCategory.BudgetCategoryId);

        await _dbService.DeleteBudgetCategory(dtoToDb);

        return Result.Success();
    }
}
