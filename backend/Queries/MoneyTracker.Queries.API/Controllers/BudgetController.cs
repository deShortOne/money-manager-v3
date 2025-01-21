using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
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

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetBudget()
    {
        var budget = await _budgetService.GetBudget(ControllerHelper.GetToken(_httpContextAccessor));

        return ControllerHelper.Convert(budget);
    }
}
