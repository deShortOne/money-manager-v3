
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class DoesCategoryExistTest : CategoryTestHelper
{
    [Fact]
    public void CategoryDoesExist()
    {
        var categoryId = 12345;
        _mockCategoryDatabase.Setup(x => x.GetCategory(categoryId)).ReturnsAsync(new CategoryEntity(categoryId, ""));

        Assert.Multiple(async () =>
        {
            Assert.True(await _categoryService.DoesCategoryExist(categoryId));

            _mockCategoryDatabase.Verify(x => x.GetCategory(categoryId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
    [Fact]
    public void CategoryNotExist()
    {
        var categoryId = 12345;
        _mockCategoryDatabase.Setup(x => x.GetCategory(categoryId)).ReturnsAsync((CategoryEntity)null);

        Assert.Multiple(async () =>
        {
            Assert.False(await _categoryService.DoesCategoryExist(categoryId));

            _mockCategoryDatabase.Verify(x => x.GetCategory(categoryId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
