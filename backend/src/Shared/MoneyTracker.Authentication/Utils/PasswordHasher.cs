using System.Security.Cryptography;
using System.Text;
using MoneyTracker.Common.Interfaces;

namespace MoneyTracker.Authentication.Utils;
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = ComputeHash(password, salt);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2)
            throw new Exception("Password stored in database failed validation.");

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var providedHash = ComputeHash(password, salt);

        return storedHash.SequenceEqual(providedHash);
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private static byte[] ComputeHash(string password, byte[] salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var combined = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            return sha256.ComputeHash(combined);
        }
    }
}
