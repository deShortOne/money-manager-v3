using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Application;
public class BudgetService : IBudgetService
{
    private readonly IUserAuthenticationService _userAuthService;
    private readonly IBudgetCommandRepository _dbService;

    public BudgetService(IUserAuthenticationService userAuthService,
        IBudgetCommandRepository dbService)
    {
        _userAuthService = userAuthService;
        _dbService = dbService;
    }

    public async Task AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoToDb = new BudgetCategoryEntity(user.Id, newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);

        await _dbService.AddBudgetCategory(dtoToDb);
    }

    public async Task EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoToDb = new EditBudgetCategoryEntity(user.Id, editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);

        await _dbService.EditBudgetCategory(dtoToDb);
    }

    public async Task DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory)
    {
        var user = await _userAuthService.DecodeToken(token);
        var dtoToDb = new DeleteBudgetCategoryEntity(user.Id, deleteBudgetCategory.BudgetGroupId, deleteBudgetCategory.BudgetCategoryId);

        await _dbService.DeleteBudgetCategory(dtoToDb);
    }
}
