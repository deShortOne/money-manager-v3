using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Shared.Core;
public interface IBudgetService
{
    Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget);
    Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory);
    Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCategory);
    Task<List<BudgetGroupDTO>> GetBudget();
}
