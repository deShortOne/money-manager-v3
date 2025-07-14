using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Contracts.Requests.Category;

namespace MoneyTracker.Commands.API.Controllers;
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
    [Route("add")]
    public Task AddCategory(NewCategoryRequest newCategory, CancellationToken cancellationToken)
    {
        return _categoryService.AddCategory(newCategory, cancellationToken);
    }

    [HttpPatch]
    [Route("edit")]
    public Task EditCategory(EditCategoryRequest editCategory, CancellationToken cancellationToken)
    {
        return _categoryService.EditCategory(editCategory, cancellationToken);
    }

    [HttpDelete]
    [Route("delete")]
    public Task DeleteCategory(DeleteCategoryRequest deleteCategory, CancellationToken cancellationToken)
    {
        return _categoryService.DeleteCategory(deleteCategory, cancellationToken);
    }
}
