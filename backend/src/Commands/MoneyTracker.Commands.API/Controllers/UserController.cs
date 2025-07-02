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
    public async Task<IActionResult> AddUser(LoginWithUsernameAndPassword usernameAndPassword, CancellationToken cancellationToken)
    {
        var result = await _authService.AddNewUser(usernameAndPassword, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser(LoginWithUsernameAndPassword usernameAndPassword, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginUser(usernameAndPassword, cancellationToken);
        return ControllerHelper.Convert(result);
    }
}
