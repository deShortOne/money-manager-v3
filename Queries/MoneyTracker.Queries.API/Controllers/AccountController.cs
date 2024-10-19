using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
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
    public Task<List<AccountResponse>> GetAllAccounts()
    {
        return _accountService.GetAccounts(ControllerHelper.GetToken(_httpContextAccessor));
    }
}
