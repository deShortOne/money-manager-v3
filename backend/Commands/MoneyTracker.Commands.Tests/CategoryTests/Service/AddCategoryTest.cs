
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class AddCategoryTest : CategoryTestHelper
{
    private readonly int _lastCategoryId = 2;
    private readonly int _newCategoryId = 3;
    private readonly string _newCategoryName = "Car : Fuel";

    [Fact]
    public async void SuccessfullyAddNewCategory()
    {
        _mockCategoryDatabase.Setup(x => x.GetLastCategoryId()).Returns(Task.FromResult(_lastCategoryId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastCategoryId)).Returns(_newCategoryId);

        var newCategoryRequest = new NewCategoryRequest(_newCategoryName);
        var newCategory = new CategoryEntity(_newCategoryId, _newCategoryName);

        await _categoryService.AddCategory(newCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.GetLastCategoryId(), Times.Once);
            _mockCategoryDatabase.Verify(x => x.AddCategory(newCategory), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastCategoryId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
