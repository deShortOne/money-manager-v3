
namespace MoneyTracker.Authentication.Interfaces;
public interface IJwtConfig
{
    string Audience { get; }
    int Expires { get; }
    string Issuer { get; }
    string Key { get; }
}
