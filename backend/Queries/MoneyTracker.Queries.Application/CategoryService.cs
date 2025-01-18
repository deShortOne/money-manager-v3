using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories;

namespace MoneyTracker.Queries.Application;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _dbService;

    public CategoryService(ICategoryRepository dbService)
    {
        _dbService = dbService;
    }

    public async Task<ResultT<List<CategoryResponse>>> GetAllCategories()
    {
        var categoriesResult = await _dbService.GetAllCategories();
        if (!categoriesResult.IsSuccess)
            return categoriesResult.Error!;

        List<CategoryResponse> res = [];
        foreach (var category in categoriesResult.Value)
        {
            res.Add(new(category.Id, category.Name));
        }
        return res;
    }
}
