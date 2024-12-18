
namespace MoneyTracker.Contracts.Requests.Budget;
public class EditBudgetCategoryRequest(int budgetCategoryId, int? budgetGroupId = null, decimal? budgetCategoryPlanned = null)
{
    public int BudgetCategoryId { get; } = budgetCategoryId;
    public int? BudgetGroupId { get; } = budgetGroupId;
    public decimal? BudgetCategoryPlanned { get; } = budgetCategoryPlanned;
}
