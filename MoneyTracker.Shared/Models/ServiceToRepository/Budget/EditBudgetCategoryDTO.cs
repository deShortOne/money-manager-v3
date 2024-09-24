namespace MoneyTracker.Shared.Models.ServiceToRepository.Budget;

public class EditBudgetCategoryDTO
{
    public EditBudgetCategoryDTO(int budgetCategoryId, int? budgetGroupId = null, decimal? budgetCategoryPlanned = null)
    {
        BudgetCategoryId = budgetCategoryId;
        BudgetGroupId = budgetGroupId;
        BudgetCategoryPlanned = budgetCategoryPlanned;
    }
    public int BudgetCategoryId { get; private set; }
    public int? BudgetGroupId { get; private set; }
    public decimal? BudgetCategoryPlanned { get; private set; }
}
