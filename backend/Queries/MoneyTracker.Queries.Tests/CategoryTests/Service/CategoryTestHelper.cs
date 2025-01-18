using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Service;
public class CategoryTestHelper
{
    public readonly Mock<ICategoryDatabase> _mockCategoryDatabase = new();

    public readonly CategoryService _budgetService;

    public CategoryTestHelper()
    {
        _budgetService = new CategoryService(_mockCategoryDatabase.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockCategoryDatabase.VerifyNoOtherCalls();
    }
}
