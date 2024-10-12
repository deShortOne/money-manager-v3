namespace MoneyTracker.Shared.Auth;

public interface IPasswordHasher
{
    bool VerifyPassword(string hashedPassword, string password, string salt);
}
