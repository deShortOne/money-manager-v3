using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Transaction;

namespace MoneyTracker.Commands.API.Controllers;
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

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddTransactions(NewTransactionRequest newTransaction, CancellationToken cancellationToken)
    {
        var result = await _registerService.AddTransaction(ControllerHelper.GetToken(_httpContextAccessor), newTransaction, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpPatch]
    [Route("edit")]
    public async Task<IActionResult> EditTransaction(EditTransactionRequest editTransaction, CancellationToken cancellationToken)
    {
        var result = await _registerService.EditTransaction(ControllerHelper.GetToken(_httpContextAccessor), editTransaction, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteTransaction(DeleteTransactionRequest deleteTransaction, CancellationToken cancellationToken)
    {
        var result = await _registerService.DeleteTransaction(ControllerHelper.GetToken(_httpContextAccessor), deleteTransaction, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpPost]
    [Route("upload-receipt")]
    public async Task<IActionResult> UploadReceipt(IFormFile uploadReceipt, CancellationToken cancellationToken)
    {
        var result = await _registerService.CreateTransactionFromReceipt(ControllerHelper.GetToken(_httpContextAccessor), uploadReceipt, cancellationToken);
        return ControllerHelper.Convert(result);
    }
}
