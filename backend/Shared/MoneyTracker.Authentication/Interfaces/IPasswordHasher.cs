
namespace MoneyTracker.Common.Interfaces;
public interface IPasswordHasher
{
    bool VerifyPassword(string hashedPassword, string password, string salt);
}
