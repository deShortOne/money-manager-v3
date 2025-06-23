using MoneyTracker.Queries.Domain.Entities.Category;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.DatabaseOnlyRepositoryService;
public class GetAllCategoriesTest : DatabaseOnlyTestHelper
{
    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var categories = new List<CategoryEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };
        _mockCategoryDatabase.Setup(x => x.GetAllCategories())
            .ReturnsAsync(categories);

        await _categoryRepositoryService.GetAllCategories();

        _mockCategoryDatabase.Verify(x => x.GetAllCategories());
        VerifyNoOtherCalls();
    }
}
