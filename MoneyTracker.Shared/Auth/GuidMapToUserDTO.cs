
namespace MoneyTracker.Shared.Auth;
public class GuidMapToUserDTO(int userId, DateTime expires)
{
    public int UserId { get; } = userId;
    public DateTime Expires { get; } = expires;
}
