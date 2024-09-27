namespace MoneyTracker.Shared.Models.ServiceToRepository.Budget;

public class DeleteBudgetCategoryDTO
{
    public DeleteBudgetCategoryDTO(int id)
    {
        BudgetCategoryId = id;
    }

    public int BudgetCategoryId { get; private set; }
}
