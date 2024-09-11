using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Services.Infrastructure;
public class BudgetDbService
{
    private readonly IBudgetDatabase _database;

    public BudgetDbService(IBudgetDatabase db)
    {
        _database = db;
    }

    public Task<List<BudgetGroupDTO>> Get()
    {
        return _database.GetBudget();
    }

    public Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget)
    {
        return _database.AddBudgetCategory(newBudget);
    }

    public Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCategory)
    {
        return _database.EditBudgetCategory(editBudgetCategory);
    }

    public Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory)
    {
        return _database.DeleteBudgetCategory(deleteBudgetCategory);
    }
}
