

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Tests.BudgetTests.Service;
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBudgetCategoryDatabase.Setup(x => x.DeleteBudgetCategory(deleteBudgetCategory));

        await _budgetService.DeleteBudgetCategory(tokenToDecode, deleteBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.DeleteBudgetCategory(deleteBudgetCategory), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}