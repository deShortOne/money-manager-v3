using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Contracts.Requests.Budget;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public sealed class EditBudgetCategoryTest : BudgetTestHelper
{
    private readonly int _userId = 509;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _categoryId = 2;

    public static TheoryData<int?, decimal?> OnlyOneItemNotNull = new() {
        {32, null},
        {null, 56},
    };

    public EditBudgetCategoryTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBudget(int? budgetGroupId, decimal? planned)
    {
        _mockUserAuthService.Setup(x => x.DecodeToken(_tokenToDecode))
            .Returns(Task.FromResult(_authedUser));

        var editBudgetCategoryRequest = new EditBudgetCategoryRequest(_categoryId, budgetGroupId, planned);
        var editBudgetCategory = new EditBudgetCategoryEntity(_userId, _categoryId, budgetGroupId, planned);

        _mockBudgetCategoryDatabase.Setup(x => x.EditBudgetCategory(editBudgetCategory));

        await _budgetService.EditBudgetCategory(_tokenToDecode, editBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(_tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.EditBudgetCategory(editBudgetCategory), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
