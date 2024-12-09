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
    public Task AddBill(NewBillRequest newBill)
    {
        return _billService.AddBill(ControllerHelper.GetToken(_httpContextAccessor), newBill);
    }

    [HttpPatch]
    [Route("edit")]
    public Task EditBill(EditBillRequest editBill)
    {
        return _billService.EditBill(ControllerHelper.GetToken(_httpContextAccessor), editBill);
    }

    [HttpDelete]
    [Route("delete")]
    public Task DeleteBill(DeleteBillRequest deleteBill)
    {
        return _billService.DeleteBill(ControllerHelper.GetToken(_httpContextAccessor), deleteBill);
    }
}
