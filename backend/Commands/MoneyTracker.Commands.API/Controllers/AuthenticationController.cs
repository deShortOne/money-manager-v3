using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Handlers;

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
    [Route("add")]
    public Task AddUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        return _authService.GetUserToken(usernameAndPassword);
    }
}
