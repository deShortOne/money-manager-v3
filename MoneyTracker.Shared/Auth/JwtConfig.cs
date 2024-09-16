
namespace MoneyTracker.Shared.Auth;
public interface IJwtConfig
{
    string Audience { get; }
    int Expires { get; }
    string Issuer { get; }
    string Key { get; }
}

public class JwtConfig(string issuer, string audience, string key, int expires) : IJwtConfig
{
    public string Issuer => issuer;

    public string Audience => audience;

    public string Key => key;

    public int Expires => expires;
}
