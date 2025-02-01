using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface ICategoryService
{
    Task<ResultT<List<CategoryResponse>>> GetAllCategories();
}
