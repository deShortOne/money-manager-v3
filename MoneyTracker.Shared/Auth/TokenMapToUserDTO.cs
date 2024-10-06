
namespace MoneyTracker.Shared.Auth;
public class TokenMapToUserDTO(int userId, DateTime expires)
{
    public int UserId { get; } = userId;
    public DateTime Expires { get; } = expires;
}
