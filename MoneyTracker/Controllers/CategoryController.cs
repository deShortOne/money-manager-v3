using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Category;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/category/")]
    public class CategoryController : ControllerBase
    {

        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<CategoryDTO>> GetCategories()
        {
            return await new Category().GetAllCategories();
        }

        [HttpPost]
        [Route("add")]
        public async Task<CategoryDTO> AddCategory([FromBody] NewCategoryDTO categoryName)
        {
            return await new Category().AddCategory(categoryName);
        }

        [HttpPut]
        [Route("edit")]
        public async Task<CategoryDTO> EditCategory([FromBody] EditCategoryDTO editCategory)
        {
            return await new Category().EditCategory(editCategory);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<bool> DeleteCategory([FromBody] DeleteCategoryDTO deleteCategory)
        {
            return await new Category().DeleteCategory(deleteCategory);
        }
    }
}
