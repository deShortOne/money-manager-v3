using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public class CategoryTestHelper
{
    public readonly Mock<ICategoryCommandRepository> _mockCategoryDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();

    public readonly CategoryService _categoryService;

    public CategoryTestHelper()
    {
        _categoryService = new CategoryService(
            _mockCategoryDatabase.Object,
            _mockIdGenerator.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockCategoryDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
    }
}
