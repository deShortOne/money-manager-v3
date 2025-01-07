using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Budget;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public sealed class EditBudgetCategoryTest : BudgetTestHelper
{
    private readonly int _userId = 509;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _categoryId = 2;

    public static TheoryData<int?, decimal?> OnlyOneItemNotNull = new() {
        {32, null},
        {null, 56},
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBudget(int? budgetGroupId, decimal? planned)
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        var editBudgetCategoryRequest = new EditBudgetCategoryRequest(_categoryId, budgetGroupId, planned);
        var editBudgetCategory = new EditBudgetCategoryEntity(_userId, _categoryId, budgetGroupId, planned);

        _mockBudgetCategoryDatabase.Setup(x => x.EditBudgetCategory(editBudgetCategory));

        await _budgetService.EditBudgetCategory(_tokenToDecode, editBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.EditBudgetCategory(editBudgetCategory), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
