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
    public async Task<IActionResult> AddBudget(NewBudgetCategoryRequest newRequest)
    {
        var result = await _budgetService.AddBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), newRequest);
        return ControllerHelper.Convert(result);
    }

    [HttpPatch]
    [Route("edit")]
    public async Task<IActionResult> EditBudget(EditBudgetCategoryRequest editRequest)
    {
        var result = await _budgetService.EditBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), editRequest);
        return ControllerHelper.Convert(result);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteBudget(DeleteBudgetCategoryRequest deleteRequest)
    {
        var result = await _budgetService.DeleteBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), deleteRequest);
        return ControllerHelper.Convert(result);
    }
}
