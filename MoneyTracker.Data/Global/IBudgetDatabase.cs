
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Data.Global
{
    public interface IBudgetDatabase
    {
        public Task<List<BudgetGroupDTO>> GetBudget();
        public Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget);
        public Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCateogry);
        public Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory);
    }
}
