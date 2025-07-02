
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Contracts.Requests.Budget;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
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
    public async Task SuccessfullyAddNewBudgetCategory()
    {
        var authedUser = new AuthenticatedUser(_userId);
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        await _budgetService.AddBudgetCategory(_tokenToDecode, _newBudgetCategoryRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.AddBudgetCategory(_newBudgetCategoryEntity, CancellationToken.None));

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Budget), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
