
namespace MoneyTracker.Authentication.Entities;
public class UserEntity(int id, string username, string password)
{
    public int Id { get; } = id;
    public string UserName { get; } = username;
    public string Password { get; } = password;

    public override bool Equals(object? obj)
    {
        var other = obj as UserEntity;
        if (other == null) return false;
        return Id == other.Id
            && UserName == other.UserName
            && Password == other.Password;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, UserName, Password);
    }
}
