using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Category;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.CacheAsideRepositoryService;
public class ResetCategoriesCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        var categorys = new List<CategoryEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockCategoryDatabase.Setup(x => x.GetAllCategories(CancellationToken.None))
            .ReturnsAsync(categorys);

        await _categoryRepositoryService.ResetCategoriesCache(CancellationToken.None);

        _mockCategoryDatabase.Verify(x => x.GetAllCategories(CancellationToken.None));
        _mockCategoryCache.Verify(x => x.SaveCategories(categorys));
        VerifyNoOtherCalls();
    }
}
