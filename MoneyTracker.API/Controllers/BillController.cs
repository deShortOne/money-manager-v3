using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.API.Controllers;
public class BillController : ControllerBase
{

    private readonly ILogger<BillController> _logger;
    private readonly IBillService _service;

    public BillController(ILogger<BillController> logger, IBillService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    [Route("get")]
    public Task<List<BillDTO>> Get()
    {
        return _service.GetAllBills();
    }

    [HttpPost]
    [Route("add")]
    public Task<List<BillDTO>> AddBill([FromBody] NewBillDTO newBill)
    {
        return _service.AddBill(newBill);
    }

    [HttpPut]
    [Route("edit")]
    public Task<List<BillDTO>> EditBill([FromBody] EditBillDTO editBill)
    {
        return _service.EditBill(editBill);
    }

    [HttpDelete]
    [Route("delete")]
    public Task<List<BillDTO>> DeleteBill([FromBody] DeleteBillDTO deleteBill)
    {
        return _service.DeleteBill(deleteBill);
    }
}
