
namespace MoneyTracker.Shared.Auth;
public class UserEntity(int id, string username)
{
    public int Id { get; } = id;
    public string UserName { get; } = username;
}
