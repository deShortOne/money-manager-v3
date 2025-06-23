
namespace MoneyTracker.Commands.Domain.Entities.Account;
public class AccountEntity(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public override bool Equals(object? obj)
    {
        var other = obj as AccountEntity;
        if (other == null) return false;
        return Id == other.Id
            && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}
