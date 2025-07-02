using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class BudgetController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBudgetService _budgetService;

    public BudgetController(IHttpContextAccessor httpContextAccessor,
        IBudgetService budgetService)
    {
        _httpContextAccessor = httpContextAccessor;
        _budgetService = budgetService;
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddBudget(NewBudgetCategoryRequest newRequest, CancellationToken cancellationToken)
    {
        var result = await _budgetService.AddBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), newRequest, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpPatch]
    [Route("edit")]
    public async Task<IActionResult> EditBudget(EditBudgetCategoryRequest editRequest, CancellationToken cancellationToken)
    {
        var result = await _budgetService.EditBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), editRequest, cancellationToken);
        return ControllerHelper.Convert(result);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteBudget(DeleteBudgetCategoryRequest deleteRequest, CancellationToken cancellationToken)
    {
        var result = await _budgetService.DeleteBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), deleteRequest, cancellationToken);
        return ControllerHelper.Convert(result);
    }
}
