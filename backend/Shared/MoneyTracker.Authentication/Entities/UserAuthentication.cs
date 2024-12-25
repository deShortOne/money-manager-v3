
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Authentication.Entities;
public class UserAuthentication(UserEntity user,
    string token,
    DateTime expiration,
    IDateTimeProvider dateTime)
{
    private readonly IDateTimeProvider _dateTime = dateTime;

    public UserEntity User { get; } = user;
    public string Token { get; } = token;
    public DateTime Expiration { get; } = expiration;

    public void ThrowIfInvalid()
    {
        if (User == null)
            throw new InvalidDataException("User not found");
        if (Token == null || Token == "")
            throw new InvalidDataException("No token found");
        if (Expiration < _dateTime.Now)
            throw new InvalidDataException("Token has expired");
    }

    public override bool Equals(object? obj)
    {
        var other = obj as UserAuthentication;
        if (other == null)
            return false;

        return User.Equals(other.User) // User == other.User doesn't work?
            && Token == other.Token
            && Expiration == other.Expiration;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(User, Token, Expiration);
    }
}