namespace MoneyTracker.Shared.Auth;
public class UnauthenticatedUser(string username)
{
    public string Username { get; } = username;
}
