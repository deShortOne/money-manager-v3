

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Tests.BudgetTests.Service;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Budget;
using Moq;

public class DeleteBudgetCategoryTest : BudgetTestHelper
{
    [Fact]
    public async void SuccessfullyDeleteBill()
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

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
