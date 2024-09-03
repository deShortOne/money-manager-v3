using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.API.Controllers;
public class BillController : ControllerBase
{

    private readonly ILogger<BillController> _logger;
    private readonly IBillDatabase _database;

    public BillController(ILogger<BillController> logger, IBillDatabase db)
    {
        _logger = logger;
        _database = db;
    }

    [HttpGet]
    [Route("get")]
    public Task<List<BillDTO>> Get()
    {
        return _database.GetBill();
    }
}
