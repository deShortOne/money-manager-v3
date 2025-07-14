using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Account;

namespace MoneyTracker.Commands.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class AccountController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountService _accountService;

    public AccountController(IHttpContextAccessor httpContextAccessor,
        IAccountService accountService)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountService = accountService;
    }

    [HttpPost]
    [Route("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAccountToUser(AddAccountToUserRequest accountToUser, CancellationToken cancellationToken)
    {
        var result = await _accountService.AddAccount(ControllerHelper.GetToken(_httpContextAccessor), accountToUser, cancellationToken);
        return ControllerHelper.Convert(result);
    }
}
