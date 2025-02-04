using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Repository.DatabaseOnlyRepositoryService;
public class DatabaseOnlyTestHelper
{
    protected Mock<ICategoryDatabase> _mockCategoryDatabase;

    protected CategoryRepository _categoryRepositoryService;

    protected DatabaseOnlyTestHelper()
    {
        _mockCategoryDatabase = new Mock<ICategoryDatabase>();

        _categoryRepositoryService = new CategoryRepository(
            _mockCategoryDatabase.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockCategoryDatabase.VerifyNoOtherCalls();
    }
}
