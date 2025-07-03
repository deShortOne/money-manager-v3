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
        _mockCategoryCache.Setup(x => x.GetAllCategories(CancellationToken.None))
            .ReturnsAsync(new List<CategoryEntity>());

        await _categoryRepositoryService.GetAllCategories(CancellationToken.None);

        _mockCategoryCache.Verify(x => x.GetAllCategories(CancellationToken.None));
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

        _mockCategoryCache.Setup(x => x.GetAllCategories(CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));
        _mockCategoryDatabase.Setup(x => x.GetAllCategories(CancellationToken.None))
            .ReturnsAsync(categories);

        await _categoryRepositoryService.GetAllCategories(CancellationToken.None);

        _mockCategoryCache.Verify(x => x.GetAllCategories(CancellationToken.None));
        _mockCategoryDatabase.Verify(x => x.GetAllCategories(CancellationToken.None));
        _mockCategoryCache.Verify(x => x.SaveCategories(categories, CancellationToken.None));
        VerifyNoOtherCalls();
    }
}
