using System.Collections.ObjectModel;

namespace MoneyTracker.Contracts.Responses.Budget;
public class BudgetGroupResponse
{
    private IList<BudgetCategoryResponse> _categories;

    public BudgetGroupResponse(string name) : this(name, 0, 0, 0, [])
    {
    }

    public BudgetGroupResponse(string name, decimal planned, decimal actual, decimal difference, IList<BudgetCategoryResponse> categories)
    {
        Name = name;
        Planned = planned;
        Actual = actual;
        Difference = difference;
        _categories = categories;
    }

    public void AddBudgetCategoryDTO(BudgetCategoryResponse newBudgetCategory)
    {
        _categories.Add(newBudgetCategory);
        Planned += newBudgetCategory.Planned;
        Actual += newBudgetCategory.Actual;
        Difference += newBudgetCategory.Difference;
    }

    public string Name { get; private set; }
    public IList<BudgetCategoryResponse> Categories
    {
        get => new ReadOnlyCollection<BudgetCategoryResponse>(_categories);
        private set => _categories = value;
    }

    public decimal Planned { get; private set; }
    public decimal Actual { get; private set; }
    public decimal Difference { get; private set; }

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetGroupResponse;

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
