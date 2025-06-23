using MoneyTracker.Authentication.Interfaces;

namespace MoneyTracker.Authentication.Authentication;
public class JwtConfig(string issuer, string audience, string key, int expires) : IJwtConfig
{
    public string Issuer => issuer;

    public string Audience => audience;

    public string Key => key;

    public int Expires => expires;
}
