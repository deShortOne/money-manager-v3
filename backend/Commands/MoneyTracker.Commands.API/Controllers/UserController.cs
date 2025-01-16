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
    public async Task<IActionResult> AddUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        var result = await _authService.AddNewUser(usernameAndPassword);
        return ControllerHelper.Convert(result);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        var result = await _authService.LoginUser(usernameAndPassword);
        return ControllerHelper.Convert(result);
    }
}
