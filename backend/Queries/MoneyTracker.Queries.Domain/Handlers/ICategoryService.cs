using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllCategories();
}
