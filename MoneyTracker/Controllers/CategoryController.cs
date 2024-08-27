using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using MoneyTracker.Controllers;

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
            return await new Category().AddCategory(categoryName);
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<CategoryDTO>> GetCategories()
        {
            return await new Category().GetAllCategories();
        }
    }
}
