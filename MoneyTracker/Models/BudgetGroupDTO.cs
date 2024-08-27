using System.Collections.Generic;

namespace MoneyTracker.API.Models
{
    public class BudgetGroupDTO : IBudget
    {
        public string Name { get; set; }
        public List<BudgetCategoryDTO> Categories { get; set; } = [];
        public decimal Planned => (from category in Categories
                                   select category.Planned).Sum();
        public decimal Actual => (from category in Categories
                                  select category.Actual).Sum();
        public decimal Difference => (from category in Categories
                                      select category.Difference).Sum();
    }
}
