using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.PlatformService.Domain;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public class CategoryTestHelper
{
    public readonly Mock<ICategoryCommandRepository> _mockCategoryDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IMessageBusClient> _mockMessageBusClient = new();

    public readonly CategoryService _categoryService;

    public CategoryTestHelper()
    {
        _categoryService = new CategoryService(
            _mockCategoryDatabase.Object,
            _mockIdGenerator.Object,
            _mockMessageBusClient.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockCategoryDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockMessageBusClient.VerifyNoOtherCalls();
    }
}
