
namespace MoneyTracker.Contracts.Requests.Budget;
public class DeleteBudgetCategoryRequest(int budgetGroupId, int budgetCategoryId)
{
    public int BudgetGroupId { get; } = budgetGroupId;
    public int BudgetCategoryId { get; } = budgetCategoryId;
}
