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

    public async Task<List<CategoryResponse>> GetAllCategories()
    {
        var dtoLisFromDb = await _dbService.GetAllCategories();
        List<CategoryResponse> res = [];
        foreach (var category in dtoLisFromDb)
        {
            res.Add(new(category.Id, category.Name));
        }
        return res;
    }
}
