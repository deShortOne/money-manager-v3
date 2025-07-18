using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Entities.Category;
using Moq;

namespace MoneyTracker.Queries.Tests.CategoryTests.Service;
public sealed class GetAllCategories : CategoryTestHelper
{
    [Fact]
    public void SuccessfullyGetBBudget()
    {
        List<CategoryEntity> categoryDatabaseReturn = [
            new(1, "name 1"),
            new(45, "briee"),
        ];
        List<CategoryResponse> expected = [
            new(1, "name 1"),
            new(45, "briee"),
        ];


        _mockCategoryDatabase.Setup(x => x.GetAllCategories(CancellationToken.None)).ReturnsAsync(categoryDatabaseReturn);

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _budgetService.GetAllCategories(CancellationToken.None));

            _mockCategoryDatabase.Verify(x => x.GetAllCategories(CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
