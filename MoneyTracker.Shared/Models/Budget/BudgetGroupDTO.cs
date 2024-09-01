namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetGroupDTO : IBudgetDTO
    {
        public string Name { get; set; }
        public List<BudgetCategoryDTO> Categories { get; set; } = [];
        public decimal Planned => (from category in Categories
                                   select category.Planned).Sum();
        public decimal Actual => (from category in Categories
                                  select category.Actual).Sum();
        public virtual decimal Difference => (from category in Categories
                                              select category.Difference).Sum();
    }
}
