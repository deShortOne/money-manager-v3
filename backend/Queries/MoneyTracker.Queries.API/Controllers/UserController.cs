using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Commands.API.Controllers;
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
}
