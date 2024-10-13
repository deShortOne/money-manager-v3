using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.Application;
public class BudgetService : IBudgetService
{
    private readonly IBudgetCommandRepository _dbService;

    public BudgetService(IBudgetCommandRepository dbService)
    {
        _dbService = dbService;
    }

    public async Task AddBudgetCategory(NewBudgetCategoryRequest newBudget)
    {
        var dtoToDb = new BudgetCategoryEntity(newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);

        await _dbService.AddBudgetCategory(dtoToDb);
    }

    public async Task EditBudgetCategory(EditBudgetCategoryRequest editBudgetCategory)
    {
        var dtoToDb = new EditBudgetCategoryEntity(editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);

        await _dbService.EditBudgetCategory(dtoToDb);
    }

    public async Task DeleteBudgetCategory(DeleteBudgetCategoryRequest deleteBudgetCategory)
    {
        var dtoToDb = new DeleteBudgetCategoryEntity(deleteBudgetCategory.BudgetGroupId, deleteBudgetCategory.BudgetCategoryId);

        await _dbService.DeleteBudgetCategory(dtoToDb);
    }
}
