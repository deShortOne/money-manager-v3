namespace MoneyTracker.Shared.Models.ControllerToService.Budget;

public class DeleteBudgetCategoryRequestDTO
{
    public DeleteBudgetCategoryRequestDTO(int id)
    {
        BudgetCategoryId = id;
    }

    public int BudgetCategoryId { get; private set; }
}
