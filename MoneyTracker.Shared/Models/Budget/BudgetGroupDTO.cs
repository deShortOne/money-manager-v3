namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetGroupDTO : IBudgetDTO
    {
        public string Name { get; set; }
        public List<BudgetCategoryDTO> Categories { get; set; } = [];
        public decimal Planned
        {
            get
            {
                return _planned;
            }
            protected set
            {
                _planned = (from category in Categories
                            select category.Planned).Sum();
            }
        }
        public decimal Actual
        {
            get
            {
                return _actual;
            }
            protected set
            {
                _actual = (from category in Categories
                           select category.Actual).Sum();
            }
        }
        public decimal Difference
        {
            get
            {
                return _difference;
            }
            protected set
            {
                _difference = (from category in Categories
                               select category.Difference).Sum();
            }
        }

        private decimal _planned;
        private decimal _actual;
        private decimal _difference;
    }
}
