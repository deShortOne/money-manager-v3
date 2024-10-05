
using MoneyTracker.Shared.Models.RepositoryToService.Category;
using MoneyTracker.Shared.Models.ServiceToRepository.Category;

namespace MoneyTracker.Shared.Data
{
    public interface ICategoryDatabase
    {
        public Task<List<CategoryEntityDTO>> GetAllCategories();
        public Task AddCategory(NewCategoryDTO categoryName);
        public Task EditCategory(EditCategoryDTO editCategoryDTO);
        public Task DeleteCategory(DeleteCategoryDTO deleteCategoryDTO);
    }
}
