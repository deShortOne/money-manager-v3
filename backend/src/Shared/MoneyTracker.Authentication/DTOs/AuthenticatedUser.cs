
namespace MoneyTracker.Authentication.DTOs;
public class AuthenticatedUser(int id)
{
    public int Id { get; } = id;

    public override bool Equals(object? obj)
    {
        var other = obj as AuthenticatedUser;
        if (other == null) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
