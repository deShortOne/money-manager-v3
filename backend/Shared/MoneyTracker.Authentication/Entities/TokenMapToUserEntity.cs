
namespace MoneyTracker.Authentication.Entities;
public class TokenMapToUserEntity(int userId, DateTime expires)
{
    public int UserId { get; } = userId;
    public DateTime Expires { get; } = expires;

    public override bool Equals(object? obj)
    {
        var other = obj as TokenMapToUserEntity;
        if (other == null) return false;
        return UserId == other.UserId &&
            Expires == other.Expires;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, Expires);
    }
}

