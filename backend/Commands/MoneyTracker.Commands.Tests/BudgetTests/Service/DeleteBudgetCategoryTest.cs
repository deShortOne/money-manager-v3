

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
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

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockBudgetCategoryDatabase.Setup(x => x.DeleteBudgetCategory(deleteBudgetCategory));

        await _budgetService.DeleteBudgetCategory(tokenToDecode, deleteBudgetCategoryRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBudgetCategoryDatabase.Verify(x => x.DeleteBudgetCategory(deleteBudgetCategory), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}