using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/category/")]
    public class CategoryController : ControllerBase
    {

        private readonly ILogger<CategoryController> _logger;
        private readonly ICategory _database;

        public CategoryController(ILogger<CategoryController> logger, ICategory db)
        {
            _logger = logger;
            _database = db;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<CategoryDTO>> GetCategories()
        {
            return _database.GetAllCategories();
        }

        [HttpPost]
        [Route("add")]
        public Task<CategoryDTO> AddCategory([FromBody] NewCategoryDTO categoryName)
        {
            return _database.AddCategory(categoryName);
        }

        [HttpPut]
        [Route("edit")]
        public Task<CategoryDTO> EditCategory([FromBody] EditCategoryDTO editCategory)
        {
            return _database.EditCategory(editCategory);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> DeleteCategory([FromBody] DeleteCategoryDTO deleteCategory)
        {
            return _database.DeleteCategory(deleteCategory);
        }
    }
}
