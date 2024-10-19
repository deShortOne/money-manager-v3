using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
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

    [HttpPost]
    [Route("get")]
    public Task<List<BillResponse>> GetAllBills()
    {
        return _billService.GetAllBills(ControllerHelper.GetToken(_httpContextAccessor));
    }
}
