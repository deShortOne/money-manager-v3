using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Category;
using MoneyTracker.Shared.Models.ServiceToController.Category;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/category/")]
    public class CategoryController : ControllerBase
    {

        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _service;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<CategoryResponseDTO>> GetCategories()
        {
            return _service.GetAllCategories();
        }

        [HttpPost]
        [Route("add")]
        public Task<CategoryResponseDTO> AddCategory([FromBody] NewCategoryRequestDTO categoryName)
        {
            return _service.AddCategory(categoryName);
        }

        [HttpPut]
        [Route("edit")]
        public Task<CategoryResponseDTO> EditCategory([FromBody] EditCategoryRequestDTO editCategory)
        {
            return _service.EditCategory(editCategory);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> DeleteCategory([FromBody] DeleteCategoryRequestDTO deleteCategory)
        {
            return _service.DeleteCategory(deleteCategory);
        }
    }
}
