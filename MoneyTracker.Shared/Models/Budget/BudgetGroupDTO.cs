using System.Collections.ObjectModel;

namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetGroupDTO
    {
        private IList<BudgetCategoryDTO> _categories;

        public BudgetGroupDTO(string name)
        {
            Name = name;
            _categories = [];
        }

        public void AddBudgetCategoryDTO(BudgetCategoryDTO newBudgetCategory)
        {
            _categories.Add(newBudgetCategory);
            Planned += newBudgetCategory.Planned;
            Actual += newBudgetCategory.Actual;
            Difference += newBudgetCategory.Difference;
        }

        public string Name { get; private set; }
        public IList<BudgetCategoryDTO> Categories
        {
            get => new ReadOnlyCollection<BudgetCategoryDTO>(_categories);
            private set => _categories = value;
        }

        public decimal Planned { get; private set; }
        public decimal Actual { get; private set; }
        public virtual decimal Difference { get; private set; }

    }
}
