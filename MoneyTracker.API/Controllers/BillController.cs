using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

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
    public Task<List<BillResponseDTO>> Get()
    {
        return _service.GetAllBills();
    }

    [HttpPost]
    [Route("add")]
    public Task<List<BillResponseDTO>> AddBill([FromBody] NewBillRequestDTO newBill)
    {
        return _service.AddBill(newBill);
    }

    [HttpPut]
    [Route("edit")]
    public Task<List<BillResponseDTO>> EditBill([FromBody] EditBillRequestDTO editBill)
    {
        return _service.EditBill(editBill);
    }

    [HttpDelete]
    [Route("delete")]
    public Task<List<BillResponseDTO>> DeleteBill([FromBody] DeleteBillRequestDTO deleteBill)
    {
        return _service.DeleteBill(deleteBill);
    }
}
