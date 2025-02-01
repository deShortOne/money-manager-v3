using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class RegisterController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRegisterService _registerService;

    public RegisterController(IHttpContextAccessor httpContextAccessor,
        IRegisterService registerService)
    {
        _httpContextAccessor = httpContextAccessor;
        _registerService = registerService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactionsResult = await _registerService.GetAllTransactions(ControllerHelper.GetToken(_httpContextAccessor));

        return ControllerHelper.Convert(transactionsResult);
    }
}
