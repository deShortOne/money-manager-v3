using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class UserController
{
    private readonly IUserService _authService;

    public UserController(IUserService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("get-token")]
    public Task<string> GetUserToken(LoginWithUsernameAndPassword usernameAndPassword)
    {
        return _authService.GetUserToken(usernameAndPassword);
    }

    [HttpPost]
    [Route("is-token-valid")]
    public Task<bool> IsTokenValid(string token)
    {
        return _authService.IsTokenValid(token);
    }
}
