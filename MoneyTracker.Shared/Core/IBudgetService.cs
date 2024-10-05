using MoneyTracker.Shared.Models.ControllerToService.Budget;
using MoneyTracker.Shared.Models.ServiceToController.Budget;

namespace MoneyTracker.Shared.Core;
public interface IBudgetService
{
    Task AddBudgetCategory(NewBudgetCategoryRequestDTO newBudget);
    Task DeleteBudgetCategory(DeleteBudgetCategoryRequestDTO deleteBudgetCategory);
    Task EditBudgetCategory(EditBudgetCategoryRequestDTO editBudgetCategory);
    Task<List<BudgetGroupResponseDTO>> GetBudget();
}
