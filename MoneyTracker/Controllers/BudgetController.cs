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
        [Route("add")]
        public async Task<BudgetCategoryDTO> AddBudget(int budgetGroupId, int categoryId, decimal planned)
        {
            return await new Budget().AddBudget(budgetGroupId, categoryId, planned);
        }
    }
}
