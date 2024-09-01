
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Data.Global
{
    public interface IBudget
    {
        public Task<IEnumerable<BudgetGroupDTO>> GetBudget();
        public Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget);
        public Task<IEnumerable<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategory editBudgetCateogry);
        public Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory);
    }
}
