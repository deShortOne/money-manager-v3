using MoneyTracker.Shared.Models.RepositoryToService.Budget;
using MoneyTracker.Shared.Models.ServiceToRepository.Budget;

namespace MoneyTracker.Shared.Data
{
    public interface IBudgetDatabase
    {
        public Task<List<BudgetGroupEntityDTO>> GetBudget();
        public Task<BudgetCategoryEntityDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget);
        public Task<List<BudgetGroupEntityDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCateogry);
        public Task<bool> DeleteBudgetCategory(DeleteBudgetCategoryDTO deleteBudgetCategory);
    }
}
