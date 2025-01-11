
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Budget;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public sealed class AddBudgetCategoryTest : BudgetTestHelper
{
    private readonly int _userId = 52;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _budgetGroupId = 1;
    private readonly int _categoryId = 2;
    private readonly decimal _planned = 123;

    private readonly NewBudgetCategoryRequest _newBudgetCategoryRequest;
    private readonly BudgetCategoryEntity _newBudgetCategoryEntity;

    public AddBudgetCategoryTest()
    {
        _newBudgetCategoryRequest = new NewBudgetCategoryRequest(_budgetGroupId, _categoryId, _planned);
        _newBudgetCategoryEntity = new BudgetCategoryEntity(_userId, _budgetGroupId, _categoryId, _planned);
    }

    [Fact]
    public async void SuccessfullyAddNewBill()
    {
        var authedUser = new AuthenticatedUser(_userId);
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(authedUser);

        await _budgetService.AddBudgetCategory(_tokenToDecode, _newBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.AddBudgetCategory(_newBudgetCategoryEntity));

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
