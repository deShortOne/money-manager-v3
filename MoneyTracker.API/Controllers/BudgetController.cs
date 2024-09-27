using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Budget;
using MoneyTracker.Shared.Models.ServiceToController.Budget;

namespace MoneyTracker.Controllers;

[ApiController]
[Route("/api/budget/")]
public class BudgetController : ControllerBase
{

    private readonly ILogger<BudgetController> _logger;
    private readonly IBudgetService _service;

    public BudgetController(ILogger<BudgetController> logger, IBudgetService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    [Route("get")]
    public Task<List<BudgetGroupResponseDTO>> Get()
    {
        return _service.GetBudget();
    }

    [HttpPost]
    [Route("category/add")]
    public Task<BudgetCategoryResponseDTO> AddBudgetCategory([FromBody] NewBudgetCategoryRequestDTO newBudget)
    {
        return _service.AddBudgetCategory(newBudget);
    }

    [HttpPut]
    [Route("category/edit")]
    public Task<List<BudgetGroupResponseDTO>> EditBudgetCategory([FromBody] EditBudgetCategoryRequestDTO editBudgetCategory)
    {
        return _service.EditBudgetCategory(editBudgetCategory);
    }

    [HttpDelete]
    [Route("category/delete")]
    public Task<bool> DeleteBudgetCategory([FromBody] DeleteBudgetCategoryRequestDTO deleteBudgetCategory)
    {
        return _service.DeleteBudgetCategory(deleteBudgetCategory);
    }
}
