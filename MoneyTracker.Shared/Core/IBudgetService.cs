using MoneyTracker.Shared.Models.ControllerToService.Budget;
using MoneyTracker.Shared.Models.ServiceToController.Budget;

namespace MoneyTracker.Shared.Core;
public interface IBudgetService
{
    Task<BudgetCategoryResponseDTO> AddBudgetCategory(NewBudgetCategoryRequestDTO newBudget);
    Task<bool> DeleteBudgetCategory(DeleteBudgetCategoryRequestDTO deleteBudgetCategory);
    Task<List<BudgetGroupResponseDTO>> EditBudgetCategory(EditBudgetCategoryRequestDTO editBudgetCategory);
    Task<List<BudgetGroupResponseDTO>> GetBudget();
}
