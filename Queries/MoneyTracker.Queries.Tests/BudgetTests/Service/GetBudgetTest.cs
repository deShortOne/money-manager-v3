using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Service;
public sealed class GetAllBillsTest : BudgetTestHelper
{
    [Fact]
    public void SuccessfullyGetBBudget()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        List<BudgetGroupEntity> budgetDatabaseReturn = [
            new("name 1", 59, 77, 99, [new("Purposefully not equal", -21, 42, 56)]),
            new("group name 2", 189, 154, 59, [new("something fun", 121, 46, 32), new("", 68, 108, 27)]),
        ];
        List<BudgetGroupResponse> expected = [
            new("name 1", 59, 77, 99, [new("Purposefully not equal", -21, 42, 56)]),
            new("group name 2", 189, 154, 59, [new("something fun", 121, 46, 32), new("", 68, 108, 27)]),
        ];

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBudgetDatabase.Setup(x => x.GetBudget(authedUser)).Returns(Task.FromResult(budgetDatabaseReturn));

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _budgetService.GetBudget(tokenToDecode));

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockBudgetDatabase.Verify(x => x.GetBudget(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
