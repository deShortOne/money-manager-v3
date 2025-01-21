using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class CategoryController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICategoryService _categoryService;

    public CategoryController(IHttpContextAccessor httpContextAccessor,
        ICategoryService categoryService)
    {
        _httpContextAccessor = httpContextAccessor;
        _categoryService = categoryService;
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategories();
        return ControllerHelper.Convert(categories);
    }
}
