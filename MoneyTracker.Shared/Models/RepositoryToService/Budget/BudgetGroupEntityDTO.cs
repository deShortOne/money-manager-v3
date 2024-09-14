using System.Collections.ObjectModel;

namespace MoneyTracker.Shared.Models.RepositoryToService.Budget
{
    public class BudgetGroupEntityDTO
    {
        private IList<BudgetCategoryEntityDTO> _categories;

        public BudgetGroupEntityDTO(string name) : this(name, 0, 0, 0, [])
        {
        }

        public BudgetGroupEntityDTO(string name, decimal planned, decimal actual, decimal difference, IList<BudgetCategoryEntityDTO> categories)
        {
            Name = name;
            Planned = planned;
            Actual = actual;
            Difference = difference;
            _categories = categories;
        }

        public void AddBudgetCategoryDTO(BudgetCategoryEntityDTO newBudgetCategory)
        {
            _categories.Add(newBudgetCategory);
            Planned += newBudgetCategory.Planned;
            Actual += newBudgetCategory.Actual;
            Difference += newBudgetCategory.Difference;
        }

        public string Name { get; private set; }
        public IList<BudgetCategoryEntityDTO> Categories
        {
            get => new ReadOnlyCollection<BudgetCategoryEntityDTO>(_categories);
            private set => _categories = value;
        }

        public decimal Planned { get; private set; }
        public decimal Actual { get; private set; }
        public decimal Difference { get; private set; }

        public override bool Equals(object? obj)
        {
            var other = obj as BudgetGroupEntityDTO;

            if (other == null)
            {
                return false;
            }

            return Name == other.Name && Planned == other.Planned &&
                Actual == other.Actual && Difference == other.Difference &&
                _categories.SequenceEqual(other._categories);
        }

        public override int GetHashCode()
        {
            return (from c in Name
                    select (int)c).Sum();
        }
    }
}
