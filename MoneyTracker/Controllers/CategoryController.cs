using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using MoneyTracker.Controllers;
using System.Net;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public CategoryController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpPost(Name = "AddCategory")]
        public async Task<int> AddBudget(string categoryName)
        {
            return await new Category().AddCategory(categoryName);
        }
    }
}
