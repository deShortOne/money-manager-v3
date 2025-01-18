
using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.PlatformService.DTOs;
public sealed class EventUpdate(AuthenticatedUser user, string name)
{
    public AuthenticatedUser User { get; } = user;
    public string Name { get; } = name;

    public override bool Equals(object? obj)
    {
        var other = obj as EventUpdate;
        if (other == null)
            return false;
        return User.Equals(other.User)
            && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(User, Name);
    }
}
