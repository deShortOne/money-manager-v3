using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.API.Controllers;
public class BillController : ControllerBase
{

    private readonly ILogger<BillController> _logger;
    private readonly IBillService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BillController(ILogger<BillController> logger, IBillService service,
            IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Route("get")]
    [Authorize]
    public Task<List<BillResponseDTO>> Get()
    {
        var token = ControllerHelper.GetToken(_httpContextAccessor);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No token provided");
        }
        return _service.GetAllBills(token);
    }

    [HttpPost]
    [Route("add")]
    [Authorize]
    public Task<List<BillResponseDTO>> AddBill([FromBody] NewBillRequestDTO newBill)
    {
        var token = ControllerHelper.GetToken(_httpContextAccessor);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No token provided");
        }
        return _service.AddBill(token, newBill);
    }

    [HttpPut]
    [Route("edit")]
    [Authorize]
    public Task<List<BillResponseDTO>> EditBill([FromBody] EditBillRequestDTO editBill)
    {
        var token = ControllerHelper.GetToken(_httpContextAccessor);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No token provided");
        }
        return _service.EditBill(token, editBill);
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize]
    public Task<List<BillResponseDTO>> DeleteBill([FromBody] DeleteBillRequestDTO deleteBill)
    {
        var token = ControllerHelper.GetToken(_httpContextAccessor);
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No token provided");
        }
        return _service.DeleteBill(token, deleteBill);
    }
}
