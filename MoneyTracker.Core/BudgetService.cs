using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Core;
public class BudgetService : IBudgetService
{
    private readonly IBudgetDatabase _dbService;

    public BudgetService(IBudgetDatabase dbService)
    {
        _dbService = dbService;
    }

    public Task<List<BudgetGroupDTO>> GetBudget()
    {
        return _dbService.GetBudget();
    }

    public Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget)
    {
        return _dbService.AddBudgetCategory(newBudget);
    }

    public Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCategory)
    {
        return _dbService.EditBudgetCategory(editBudgetCategory);
    }

    public Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory)
    {
        return _dbService.DeleteBudgetCategory(deleteBudgetCategory);
    }
}
