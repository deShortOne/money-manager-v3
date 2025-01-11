using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetCommandRepository _dbService;
    private readonly IUserCommandRepository _userRepository;

    public BudgetService(
        IBudgetCommandRepository dbService,
        IUserCommandRepository userRepository)
    {
        _dbService = dbService;
        _userRepository = userRepository;
    }

    public async Task AddBudgetCategory(string token, NewBudgetCategoryRequest newBudget)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoToDb = new BudgetCategoryEntity(user.Id, newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);

        await _dbService.AddBudgetCategory(dtoToDb);
    }

    public async Task EditBudgetCategory(string token, EditBudgetCategoryRequest editBudgetCategory)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoToDb = new EditBudgetCategoryEntity(user.Id, editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);

        await _dbService.EditBudgetCategory(dtoToDb);
    }

    public async Task DeleteBudgetCategory(string token, DeleteBudgetCategoryRequest deleteBudgetCategory)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            throw new InvalidDataException("Token not found");
        userAuth.CheckValidation();
        
        var user = new AuthenticatedUser(userAuth.User.Id);
        var dtoToDb = new DeleteBudgetCategoryEntity(user.Id, deleteBudgetCategory.BudgetGroupId, deleteBudgetCategory.BudgetCategoryId);

        await _dbService.DeleteBudgetCategory(dtoToDb);
    }
}
