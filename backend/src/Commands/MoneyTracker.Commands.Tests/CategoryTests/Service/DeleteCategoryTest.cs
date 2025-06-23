
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Contracts.Requests.Category;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class DeleteCategoryTest : CategoryTestHelper
{
    private readonly int _categoryId = 2;

    [Fact]
    public async Task DeleteCategory()
    {
        var editCategoryRequest = new DeleteCategoryRequest(_categoryId);

        await _categoryService.DeleteCategory(editCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.DeleteCategory(_categoryId), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                It.Is<EventUpdate>(x => x.Name == DataTypes.Category), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
