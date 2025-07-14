using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Application;
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepositoryService _categoryRepository;

    public CategoryService(ICategoryRepositoryService dbService)
    {
        _categoryRepository = dbService;
    }

    public async Task<ResultT<List<CategoryResponse>>> GetAllCategories(CancellationToken cancellationToken)
    {
        var categoriesResult = await _categoryRepository.GetAllCategories(cancellationToken);
        if (categoriesResult.HasError)
            return categoriesResult.Error!;

        List<CategoryResponse> res = [];
        foreach (var category in categoriesResult.Value)
        {
            res.Add(new(category.Id, category.Name));
        }
        return res;
    }
}
