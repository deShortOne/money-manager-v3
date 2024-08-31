namespace MoneyTracker.API.Models.Budget
{
    public class EditBudgetCategory
    {
        public int BudgetCategoryId { get; set; }
        public int? BudgetGroupId { get; set; }
        public decimal? BudgetCategoryPlanned { get; set; }
    }
}
