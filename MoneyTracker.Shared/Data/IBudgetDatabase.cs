using MoneyTracker.Shared.Models.RepositoryToService.Budget;
using MoneyTracker.Shared.Models.ServiceToRepository.Budget;

namespace MoneyTracker.Shared.Data
{
    public interface IBudgetDatabase
    {
        public Task<List<BudgetGroupEntityDTO>> GetBudget();
        public Task AddBudgetCategory(NewBudgetCategoryDTO newBudget);
        public Task EditBudgetCategory(EditBudgetCategoryDTO editBudgetCateogry);
        public Task DeleteBudgetCategory(DeleteBudgetCategoryDTO deleteBudgetCategory);
    }
}
