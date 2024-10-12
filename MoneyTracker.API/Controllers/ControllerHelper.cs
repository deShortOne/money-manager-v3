
namespace MoneyTracker.API.Controllers;
public class ControllerHelper
{
    public static string GetToken(IHttpContextAccessor httpContextAccessor)
    {
        var authHeader = httpContextAccessor.HttpContext.Request
            .Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return "";
    }
}
