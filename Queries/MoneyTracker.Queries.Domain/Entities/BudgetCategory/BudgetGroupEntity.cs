using System.Collections.ObjectModel;

namespace MoneyTracker.Queries.Domain.Entities.BudgetCategory;
public class BudgetGroupEntity
{
    private IList<BudgetCategoryEntity> _categories;

    public BudgetGroupEntity(string name) : this(name, 0, 0, 0, [])
    {
    }

    public BudgetGroupEntity(string name, decimal planned, decimal actual, decimal difference, IList<BudgetCategoryEntity> categories)
    {
        Name = name;
        Planned = planned;
        Actual = actual;
        Difference = difference;
        _categories = categories;
    }

    public void AddBudgetCategory(BudgetCategoryEntity newBudgetCategory)
    {
        _categories.Add(newBudgetCategory);
        Planned += newBudgetCategory.Planned;
        Actual += newBudgetCategory.Actual;
        Difference += newBudgetCategory.Difference;
    }

    public string Name { get; private set; }
    public IList<BudgetCategoryEntity> Categories
    {
        get => new ReadOnlyCollection<BudgetCategoryEntity>(_categories);
        private set => _categories = value;
    }

    public decimal Planned { get; private set; }
    public decimal Actual { get; private set; }
    public decimal Difference { get; private set; }

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetGroupEntity;

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
