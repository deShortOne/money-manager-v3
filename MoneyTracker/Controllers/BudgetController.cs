using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using System.Net;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BudgetController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public BudgetController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBudget")]
        public Task<IEnumerable<BudgetGroup>> Get()
        {
            var budget = new Budget();
            return budget.GetBudget();
        }

        [HttpPost(Name = "AddBudget")]
        public HttpStatusCode AddBudget(int budgetGroupId, string categoryName, decimal planned)
        {
            new Budget().AddBudget(budgetGroupId, categoryName, planned);
            return HttpStatusCode.OK;
        }
    }
}
