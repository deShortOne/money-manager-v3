using MoneyTracker.Contracts.Requests.Category;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface ICategoryService
{
    Task AddCategory(NewCategoryRequest newCategory);
    Task DeleteCategory(DeleteCategoryRequest deleteCategory);
    Task EditCategory(EditCategoryRequest editCategory);
}
