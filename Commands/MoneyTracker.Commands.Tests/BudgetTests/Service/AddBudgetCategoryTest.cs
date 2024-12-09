using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.Contracts.Requests.Budget;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public sealed class AddBudgetCategoryTest : BudgetTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _budgetGroupId = 1;
    private readonly int _categoryId = 2;
    private readonly decimal _planned = 123;

    private readonly NewBudgetCategoryRequest _newBudgetCategoryRequest;
    private readonly BudgetCategoryEntity _newBudgetCategoryEntity;

    public AddBudgetCategoryTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
        _newBudgetCategoryRequest = new NewBudgetCategoryRequest(_budgetGroupId, _categoryId, _planned);
        _newBudgetCategoryEntity = new BudgetCategoryEntity(_userId, _budgetGroupId, _categoryId, _planned);
    }

    [Fact]
    public async void SuccessfullyAddNewBill()
    {
        _mockUserAuthService.Setup(x => x.DecodeToken(_tokenToDecode))
            .Returns(Task.FromResult(_authedUser));

        await _budgetService.AddBudgetCategory(_tokenToDecode, _newBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(_tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.AddBudgetCategory(_newBudgetCategoryEntity));

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
