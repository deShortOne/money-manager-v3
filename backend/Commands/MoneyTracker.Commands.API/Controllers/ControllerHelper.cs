
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.API.Controllers;
[ExcludeFromCodeCoverage]
public class ControllerHelper
{
    public static string GetToken(IHttpContextAccessor httpContextAccessor)
    {
        var authHeader = httpContextAccessor.HttpContext?.Request
            .Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }
        return "";
    }

    public static IActionResult Convert(Result result)
    {
        if (result.IsSuccess)
            return new OkResult();
        if (result.Error != null)
        {
            switch (result.Error.ErrorType)
            {
                case ErrorType.NotFound:
                    return new ContentResult
                    {
                        Content = result.Error.Description,
                        ContentType = "text/plain",
                        StatusCode = StatusCodes.Status404NotFound,
                    };
                case ErrorType.AccessUnAuthorised:
                    return new ContentResult
                    {
                        Content = result.Error.Description,
                        ContentType = "text/plain",
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };
                case ErrorType.Validation:
                    return new ContentResult
                    {
                        Content = result.Error.Description,
                        ContentType = "text/plain",
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                case ErrorType.Failure:
                default:
                    return new ContentResult
                    {
                        Content = result.Error.Description,
                        ContentType = "text/plain",
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
            }
        }
        return new ContentResult
        {
            Content = "Critical: result not successful but no error was found",
            ContentType = "text/plain",
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}
