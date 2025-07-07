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
    public async Task<IActionResult> GetAllTransactions(CancellationToken cancellationToken)
    {
        var transactionsResult = await _registerService.GetAllTransactions(ControllerHelper.GetToken(_httpContextAccessor), cancellationToken);

        return ControllerHelper.Convert(transactionsResult);
    }

    [HttpPost]
    [Route("get-temporary-transaction")]
    public async Task<IActionResult> GetTemporaryTransactions(string filename, CancellationToken cancellationToken)
    {
        var token = ControllerHelper.GetToken(_httpContextAccessor);
        var transactionsResult = await _registerService.GetTransactionFromReceipt(token, filename, cancellationToken);

        return ControllerHelper.Convert(transactionsResult);
    }
}
