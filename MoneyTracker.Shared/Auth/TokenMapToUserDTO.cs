
namespace MoneyTracker.Shared.Auth;
public class TokenMapToUserDTO(int userId, DateTime expires)
{
    public int UserId { get; } = userId;
    public DateTime Expires { get; } = expires;

    public override bool Equals(object? obj)
    {
        var other = obj as TokenMapToUserDTO;
        if (other == null) return false;
        return UserId == other.UserId &&
            Expires == other.Expires;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, Expires);
    }
}
