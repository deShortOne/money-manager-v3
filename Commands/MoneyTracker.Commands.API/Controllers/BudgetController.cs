using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Budget;

namespace MoneyTracker.Commands.API.Controllers;
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
    [Route("add")]
    public Task AddBudget(NewBudgetCategoryRequest newRequest)
    {
        return _budgetService.AddBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), newRequest);
    }

    [HttpPatch]
    [Route("edit")]
    public Task EditBudget(EditBudgetCategoryRequest editRequest)
    {
        return _budgetService.EditBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), editRequest);
    }

    [HttpDelete]
    [Route("delete")]
    public Task DeleteBudget(DeleteBudgetCategoryRequest deleteRequest)
    {
        return _budgetService.DeleteBudgetCategory(ControllerHelper.GetToken(_httpContextAccessor), deleteRequest);
    }
}
