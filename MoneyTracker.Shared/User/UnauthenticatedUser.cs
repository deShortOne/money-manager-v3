
namespace MoneyTracker.Shared.User;
public class UnauthenticatedUser(string username)
{
    public string Username { get; } = username;
}
