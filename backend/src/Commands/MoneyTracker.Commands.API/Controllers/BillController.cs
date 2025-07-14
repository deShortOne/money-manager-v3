using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Bill;

namespace MoneyTracker.Commands.API.Controllers;
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

    [HttpPost]
    [Route("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddBill(NewBillRequest newBill, CancellationToken cancellationToken)
    {
        var result = await _billService.AddBill(ControllerHelper.GetToken(_httpContextAccessor), newBill, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpPatch]
    [Route("edit")]
    public async Task<IActionResult> EditBill(EditBillRequest editBill, CancellationToken cancellationToken)
    {
        var result = await _billService.EditBill(ControllerHelper.GetToken(_httpContextAccessor), editBill, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteBill(DeleteBillRequest deleteBill, CancellationToken cancellationToken)
    {
        var result = await _billService.DeleteBill(ControllerHelper.GetToken(_httpContextAccessor), deleteBill, cancellationToken);
        return ControllerHelper.Convert(result);
    }
}
