using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class BillController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBillService _billService;

    public BillController(IHttpContextAccessor httpContextAccessor,
        IBillService billService)
    {
        _httpContextAccessor = httpContextAccessor;
        _billService = billService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetAllBills(CancellationToken cancellationToken)
    {
        var billsResult = await _billService.GetAllBills(ControllerHelper.GetToken(_httpContextAccessor), cancellationToken);
        return ControllerHelper.Convert(billsResult);
    }

    [HttpGet]
    [Route("get-all-frequency-names")]
    public Task<List<string>> GetAllFrequencyNames(CancellationToken cancellationToken)
    {
        return _billService.GetAllFrequencyNames();
    }
}
