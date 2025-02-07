using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Contracts.Requests.Budget;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public class DeleteBudgetCategoryTest : BudgetTestHelper
{
    [Fact]
    public async Task SuccessfullyDeleteBudgetCategory()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var budgetGroupId = 12;
        var categoryId = 34;

        var deleteBudgetCategoryRequest = new DeleteBudgetCategoryRequest(budgetGroupId, categoryId);
        var deleteBudgetCategory = new DeleteBudgetCategoryEntity(userId, budgetGroupId, categoryId);

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        _mockBudgetCategoryDatabase.Setup(x => x.DeleteBudgetCategory(deleteBudgetCategory));

        await _budgetService.DeleteBudgetCategory(tokenToDecode, deleteBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.DeleteBudgetCategory(deleteBudgetCategory), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Budget), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
