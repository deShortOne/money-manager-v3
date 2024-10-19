using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
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
    [Route("get")]
    public Task<List<BudgetGroupResponse>> GetBudget()
    {
        return _budgetService.GetBudget();
    }
}
