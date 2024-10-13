using MoneyTracker.Common.Interfaces;

namespace MoneyTracker.Authentication.Utils;
public class PasswordHasher : IPasswordHasher
{
    public bool VerifyPassword(string hashedPassword, string password, string salt)
    {
        return hashedPassword == password;
    }
}
