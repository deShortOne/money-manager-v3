using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Contracts.Requests.Budget;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
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
    public async Task EditBudget(int? budgetGroupId, decimal? planned)
    {
        var authedUser = new AuthenticatedUser(_userId);
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        var editBudgetCategoryRequest = new EditBudgetCategoryRequest(_categoryId, budgetGroupId, planned);
        var editBudgetCategory = new EditBudgetCategoryEntity(_userId, _categoryId, budgetGroupId, planned);

        _mockBudgetCategoryDatabase.Setup(x => x.EditBudgetCategory(editBudgetCategory, CancellationToken.None));

        await _budgetService.EditBudgetCategory(_tokenToDecode, editBudgetCategoryRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.EditBudgetCategory(editBudgetCategory, CancellationToken.None), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Budget), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
