
namespace MoneyTracker.Authentication.DTOs;
public class LoginWithUsernameAndPassword(string username, string password)
{
    public string Username { get; } = username;
    public string Password { get; } = password;

    public override bool Equals(object? obj)
    {
        var other = obj as LoginWithUsernameAndPassword;
        if (other == null) return false;
        return Username == other.Username && Password == other.Password;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Password);
    }
}
