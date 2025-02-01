using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.CacheAsideRepositoryService;
public class CacheAsideTestHelper
{
    protected Mock<ICategoryDatabase> _mockCategoryDatabase;
    protected Mock<ICategoryCache> _mockCategoryCache;

    protected CategoryRepository _categoryRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockCategoryDatabase = new Mock<ICategoryDatabase>();
        _mockCategoryCache = new Mock<ICategoryCache>();

        _categoryRepositoryService = new CategoryRepository(
            _mockCategoryDatabase.Object,
            _mockCategoryCache.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockCategoryDatabase.VerifyNoOtherCalls();
        _mockCategoryCache.VerifyNoOtherCalls();
    }
}
