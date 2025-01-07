
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
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        await _budgetService.AddBudgetCategory(_tokenToDecode, _newBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.AddBudgetCategory(_newBudgetCategoryEntity));

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
