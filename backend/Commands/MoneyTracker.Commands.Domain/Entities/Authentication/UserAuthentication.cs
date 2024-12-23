
using MoneyTracker.Authentication.Entities;

public class UserAuthentication(UserEntity user, string token, DateTime expiration)
{
    public UserEntity User { get; } = user;
    public string Token { get; } = token;
    public DateTime Expiration { get; } = expiration;

    public override bool Equals(object? obj)
    {
        var other = obj as UserAuthentication;
        if (other == null)
            return false;

        return User == other.User
            && Token == other.Token
            && Expiration == other.Expiration;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(User, Token, Expiration);
    }
}