using MoneyTracker.Contracts.Requests.Category;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface ICategoryService
{
    Task AddCategory(NewCategoryRequest newCategory, CancellationToken cancellationToken);
    Task DeleteCategory(DeleteCategoryRequest deleteCategory, CancellationToken cancellationToken);
    Task EditCategory(EditCategoryRequest editCategory, CancellationToken cancellationToken);
    Task<bool> DoesCategoryExist(int categoryId, CancellationToken cancellationToken);
}
