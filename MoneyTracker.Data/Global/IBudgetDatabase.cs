
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Data.Global
{
    public interface IBudgetDatabase
    {
        public Task<List<BudgetGroupDTO>> GetBudget();
        public Task<BudgetCategoryDTO> AddBudgetCategory(EditBudgetCategoryDTO newBudget);
        public Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategory editBudgetCateogry);
        public Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory);
    }
}
