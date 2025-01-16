using MoneyTracker.Common.Result;

namespace MoneyTracker.Authentication.Entities;
public interface IUserAuthentication
{
    DateTime Expiration { get; }
    string Token { get; }
    UserEntity User { get; }

    Result CheckValidation();
    bool Equals(object? obj);
    int GetHashCode();
}