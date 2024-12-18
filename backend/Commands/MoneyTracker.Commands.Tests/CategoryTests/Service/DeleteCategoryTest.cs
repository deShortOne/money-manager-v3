
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class DeleteCategoryTest : CategoryTestHelper
{
    private readonly int _categoryId = 2;

    [Fact]
    public async void DeleteCategory()
    {
        var editCategoryRequest = new DeleteCategoryRequest(_categoryId);

        await _categoryService.DeleteCategory(editCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.DeleteCategory(_categoryId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
