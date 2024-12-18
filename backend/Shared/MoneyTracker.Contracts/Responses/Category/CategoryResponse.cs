
namespace MoneyTracker.Contracts.Responses.Category;
public class CategoryResponse
{
    public CategoryResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }

    public override bool Equals(object? obj)
    {
        var other = obj as CategoryResponse;

        if (other == null)
        {
            return false;
        }
        return Id == other.Id && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
