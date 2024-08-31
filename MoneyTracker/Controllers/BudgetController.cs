using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("/api/budget/")]
    public class BudgetController : ControllerBase
    {

        private readonly ILogger<BudgetController> _logger;

        public BudgetController(ILogger<BudgetController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public Task<IEnumerable<BudgetGroupDTO>> Get()
        {
            var budget = new Budget();
            return budget.GetBudget();
        }

        [HttpPost]
        [Route("category/add")]
        public async Task<BudgetCategoryDTO> AddBudgetCategory([FromBody] NewBudgetCategoryDTO newBudget)
        {
            return await new Budget().AddBudgetCategory(newBudget);
        }

        [HttpPut]
        [Route("category/edit")]
        public async Task<IEnumerable<BudgetGroupDTO>> EditBudgetCategory([FromBody] EditBudgetCategory editBudgetCategory)
        {
            return await new Budget().EditBudgetCategory(editBudgetCategory);
        }

        [HttpDelete]
        [Route("category/delete")]
        public async Task<bool> DeleteBudgetCategory([FromBody] DeleteBudgetCategory deleteBudgetCategory)
        {
            return await new Budget().DeleteBudgetCategory(deleteBudgetCategory);
        }
    }
}
