using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models.Budget;
using System.Net;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("/api/budget/")]
    public class BudgetController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public BudgetController(ILogger<WeatherForecastController> logger)
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
            return false;
        }
    }
}
