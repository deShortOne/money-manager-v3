
namespace MoneyTracker.Shared.User;
public class AuthenticatedUser(int userId)
{
    public int UserId { get; } = userId;

    public override bool Equals(object? obj)
    {
        var other = obj as AuthenticatedUser;
        if (other == null) return false;
        return UserId == other.UserId;
    }

    public override int GetHashCode()
    {
        return UserId;
    }
}
