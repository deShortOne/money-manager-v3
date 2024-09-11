using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.Budget;

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
        [Route("get")]
        public Task<List<BudgetGroupDTO>> Get()
        {
            return _service.GetBudget();
        }

        [HttpPost]
        [Route("category/add")]
        public Task<BudgetCategoryDTO> AddBudgetCategory([FromBody] NewBudgetCategoryDTO newBudget)
        {
            return _service.AddBudgetCategory(newBudget);
        }

        [HttpPut]
        [Route("category/edit")]
        public Task<List<BudgetGroupDTO>> EditBudgetCategory([FromBody] EditBudgetCategoryDTO editBudgetCategory)
        {
            return _service.EditBudgetCategory(editBudgetCategory);
        }

        [HttpDelete]
        [Route("category/delete")]
        public Task<bool> DeleteBudgetCategory([FromBody] DeleteBudgetCategory deleteBudgetCategory)
        {
            return _service.DeleteBudgetCategory(deleteBudgetCategory);
        }
    }
}
