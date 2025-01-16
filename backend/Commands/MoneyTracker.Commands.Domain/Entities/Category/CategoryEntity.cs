
namespace MoneyTracker.Commands.Domain.Entities.Category;
public class CategoryEntity(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public override bool Equals(object? obj)
    {
        var other = obj as CategoryEntity;
        if (other == null) return false;
        return Id == other.Id && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
