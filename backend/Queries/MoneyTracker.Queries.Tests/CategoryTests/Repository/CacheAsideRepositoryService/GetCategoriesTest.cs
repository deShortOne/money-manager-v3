using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Category;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.CacheAsideRepositoryService;
public class GetAllCategoriesTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataInCacheWontCallOffToDatabase()
    {
        _mockCategoryCache.Setup(x => x.GetAllCategories())
            .ReturnsAsync(new List<CategoryEntity>());

        await _categoryRepositoryService.GetAllCategories();

        _mockCategoryCache.Verify(x => x.GetAllCategories());
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var categories = new List<CategoryEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockCategoryCache.Setup(x => x.GetAllCategories())
            .ReturnsAsync(Error.NotFound("", ""));
        _mockCategoryDatabase.Setup(x => x.GetAllCategories())
            .ReturnsAsync(categories);

        await _categoryRepositoryService.GetAllCategories();

        _mockCategoryCache.Verify(x => x.GetAllCategories());
        _mockCategoryDatabase.Verify(x => x.GetAllCategories());
        _mockCategoryCache.Verify(x => x.SaveCategories(categories));
        VerifyNoOtherCalls();
    }
}
