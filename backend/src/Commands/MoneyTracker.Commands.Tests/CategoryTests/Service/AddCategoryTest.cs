
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class AddCategoryTest : CategoryTestHelper
{
    private readonly int _lastCategoryId = 2;
    private readonly int _newCategoryId = 3;
    private readonly string _newCategoryName = "Car : Fuel";

    [Fact]
    public async Task SuccessfullyAddNewCategory()
    {
        _mockCategoryDatabase.Setup(x => x.GetLastCategoryId(CancellationToken.None)).Returns(Task.FromResult(_lastCategoryId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastCategoryId)).Returns(_newCategoryId);

        var newCategoryRequest = new NewCategoryRequest(_newCategoryName);
        var newCategory = new CategoryEntity(_newCategoryId, _newCategoryName);

        await _categoryService.AddCategory(newCategoryRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.GetLastCategoryId(CancellationToken.None), Times.Once);
            _mockCategoryDatabase.Verify(x => x.AddCategory(newCategory, CancellationToken.None), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastCategoryId), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                It.Is<EventUpdate>(x => x.Name == DataTypes.Category), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
