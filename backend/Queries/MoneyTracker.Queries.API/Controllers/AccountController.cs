using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
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
    [Route("get")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAccounts(ControllerHelper.GetToken(_httpContextAccessor));

        return ControllerHelper.Convert(accounts);
    }
}
