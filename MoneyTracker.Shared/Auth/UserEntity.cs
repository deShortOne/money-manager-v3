
namespace MoneyTracker.Shared.Auth;
public class UserEntity(int id, string username, string password)
{
    public int Id { get; } = id;
    public string UserName { get; } = username;
    public string Password { get; } = password;
}
