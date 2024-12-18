
using System.Runtime.CompilerServices;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Contracts.Requests.Category;
using Moq;

namespace MoneyTracker.Commands.Tests.CategoryTests.Service;
public sealed class EditCategoryTest : CategoryTestHelper
{
    private readonly int _categoryId = 2;
    private readonly string _newCategoryName = "Car : MOT";

    [Fact]
    public async void EditCategoryName()
    {
        var editCategoryRequest = new EditCategoryRequest(_categoryId, _newCategoryName);
        var editCategory = new EditCategoryEntity(_categoryId, _newCategoryName);

        await _categoryService.EditCategory(editCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockCategoryDatabase.Verify(x => x.EditCategory(editCategory), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
