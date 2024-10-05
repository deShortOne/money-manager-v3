namespace MoneyTracker.Shared.Auth;
public class LoginWithUsernameAndPassword(string username)
{
    public string Username { get; } = username;

    public override bool Equals(object? obj)
    {
        var other = obj as LoginWithUsernameAndPassword;
        if (other == null) return false;
        return Username == other.Username;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username);
    }
}
