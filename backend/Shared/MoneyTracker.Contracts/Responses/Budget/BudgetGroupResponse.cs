using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace MoneyTracker.Contracts.Responses.Budget;
public class BudgetGroupResponse
{
    private IList<BudgetCategoryResponse> _categories;

    public BudgetGroupResponse(int id, string name) : this(id, name, 0, 0, 0, [])
    {
    }

    public BudgetGroupResponse(int id, string name, decimal planned, decimal actual, decimal difference, IList<BudgetCategoryResponse> categories)
    {
        Id = id;
        Name = name;
        Planned = planned;
        Actual = actual;
        Difference = difference;
        _categories = categories;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public IList<BudgetCategoryResponse> Categories
    {
        get => new ReadOnlyCollection<BudgetCategoryResponse>(_categories);
        private set => _categories = value;
    }

    public decimal Planned { get; private set; }
    public decimal Actual { get; private set; }
    public decimal Difference { get; private set; }

    public void AddBudgetCategoryDTO(BudgetCategoryResponse newBudgetCategory)
    {
        _categories.Add(newBudgetCategory);
        Planned += newBudgetCategory.Planned;
        Actual += newBudgetCategory.Actual;
        Difference += newBudgetCategory.Difference;
    }

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetGroupResponse;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id
            && Name == other.Name
            && Planned == other.Planned
            && Actual == other.Actual
            && Difference == other.Difference
            && Categories.SequenceEqual(other.Categories);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Planned, Actual, Difference, Categories);
    }
}
