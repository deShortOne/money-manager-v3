namespace MoneyTracker.Shared.Models.Budget
{
    public class EditBudgetCategory
    {
        public EditBudgetCategory(int budgetCategoryId, int? budgetGroupId = null, decimal? budgetCategoryPlanned = null)
        {
            BudgetCategoryId = budgetCategoryId;
            BudgetGroupId = budgetGroupId;
            BudgetCategoryPlanned = budgetCategoryPlanned;
        }
        public int BudgetCategoryId { get; private set; }
        public int? BudgetGroupId { get; private set; }
        public decimal? BudgetCategoryPlanned { get; private set; }
    }
}
