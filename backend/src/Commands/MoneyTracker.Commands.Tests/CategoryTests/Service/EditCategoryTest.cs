
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class EditCategoryTest : CategoryTestHelper
{
    private readonly int _categoryId = 2;
    private readonly string _newCategoryName = "Car : MOT";

    [Fact]
    public async Task EditCategoryName()
    {
        var editCategoryRequest = new EditCategoryRequest(_categoryId, _newCategoryName);
        var editCategory = new EditCategoryEntity(_categoryId, _newCategoryName);

        await _categoryService.EditCategory(editCategoryRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.EditCategory(editCategory, CancellationToken.None), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                It.Is<EventUpdate>(x => x.Name == DataTypes.Category), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
