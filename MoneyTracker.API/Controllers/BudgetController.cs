using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Budget;
using MoneyTracker.Shared.Models.ServiceToController.Budget;

namespace MoneyTracker.Controllers
{
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
        [Authorize]
        [Route("get")]
        public Task<List<BudgetGroupResponseDTO>> Get()
        {
            return _service.GetBudget();
        }

        [HttpPost]
        [Authorize]
        [Route("category/add")]
        public Task AddBudgetCategory([FromBody] NewBudgetCategoryRequestDTO newBudget)
        {
            return _service.AddBudgetCategory(newBudget);
        }

        [HttpPut]
        [Authorize]
        [Route("category/edit")]
        public Task EditBudgetCategory([FromBody] EditBudgetCategoryRequestDTO editBudgetCategory)
        {
            return _service.EditBudgetCategory(editBudgetCategory);
        }

        [HttpDelete]
        [Authorize]
        [Route("category/delete")]
        public Task DeleteBudgetCategory([FromBody] DeleteBudgetCategoryRequestDTO deleteBudgetCategory)
        {
            return _service.DeleteBudgetCategory(deleteBudgetCategory);
        }
    }
}
