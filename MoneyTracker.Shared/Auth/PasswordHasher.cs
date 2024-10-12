
namespace MoneyTracker.Shared.Auth;
public class PasswordHasher : IPasswordHasher
{
    public bool VerifyPassword(string hashedPassword, string password, string salt)
    {
        return hashedPassword == password;
    }
}
