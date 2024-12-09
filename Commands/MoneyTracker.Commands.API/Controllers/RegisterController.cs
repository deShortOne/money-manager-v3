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
    public Task AddTransactions(NewTransactionRequest newTransaction)
    {
        return _registerService.AddTransaction(ControllerHelper.GetToken(_httpContextAccessor), newTransaction);
    }

    [HttpPatch]
    [Route("edit")]
    public Task EditTransaction(EditTransactionRequest editTransaction)
    {
        return _registerService.EditTransaction(ControllerHelper.GetToken(_httpContextAccessor), editTransaction);
    }

    [HttpDelete]
    [Route("delete")]
    public Task DeleteTransaction(DeleteTransactionRequest deleteTransaction)
    {
        return _registerService.DeleteTransaction(ControllerHelper.GetToken(_httpContextAccessor), deleteTransaction);
    }
}
