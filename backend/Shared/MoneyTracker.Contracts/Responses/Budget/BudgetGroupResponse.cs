using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text.Json.Serialization;

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

    [JsonPropertyName("id")]
    public int Id { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("categories")]
    public IList<BudgetCategoryResponse> Categories
    {
        get => new ReadOnlyCollection<BudgetCategoryResponse>(_categories);
        private set => _categories = value;
    }

    [JsonPropertyName("planned")]
    public decimal Planned { get; private set; }

    [JsonPropertyName("actual")]
    public decimal Actual { get; private set; }

    [JsonPropertyName("difference")]
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
