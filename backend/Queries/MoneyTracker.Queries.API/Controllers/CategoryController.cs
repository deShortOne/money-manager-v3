using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Responses.Category;
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

    [HttpPost]
    [Route("get")]
    public Task<List<CategoryResponse>> GetAllCategories()
    {
        return _categoryService.GetAllCategories();
    }
}
