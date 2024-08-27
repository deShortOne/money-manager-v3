using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using MoneyTracker.Controllers;
using System.Net;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/category/")]
    public class CategoryController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public CategoryController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        [Route("add")]
        public async Task<int> AddCategory(string categoryName)
        {
            return await new MoneyTracker.API.Database.Category().AddCategory(categoryName);
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<MoneyTracker.API.Models.Category>> GetCategories()
        {
            return await new MoneyTracker.API.Database.Category().GetAllCategories();
        }
    }
}
