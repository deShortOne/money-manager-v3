namespace MoneyTracker.API.Models.Budget
{
    public class NewBudgetCategoryDTO
    {
        public int BudgetGroupId { get; set; }
        public int CategoryId { get; set; }
        public decimal Planned { get; set; }
    }
}
