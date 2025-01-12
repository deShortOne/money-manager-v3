
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Authentication.Entities;
public class UserAuthentication(UserEntity user,
    string token,
    DateTime expiration,
    IDateTimeProvider dateTime) : IUserAuthentication
{
    private readonly IDateTimeProvider _dateTime = dateTime;

    public UserEntity User { get; } = user;
    public string Token { get; } = token;
    public DateTime Expiration { get; } = expiration;

    public Result CheckValidation()
    {
        if (User == null)
            return Result.Failure(Error.Validation("UserAuthentication.Validation", "No user found"));
        if (Token == null || Token == "")
            return Result.Failure(Error.Validation("UserAuthentication.Validation", "No token found"));
        if (Expiration < _dateTime.Now)
            return Result.Failure(Error.Validation("UserAuthentication.Validation", "Token has expired"));
        return Result.Success();
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
