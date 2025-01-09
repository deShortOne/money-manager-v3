using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Responses.Budget;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Service;
public sealed class GetAllBillsTest : BudgetTestHelper
{
    [Fact]
    public void SuccessfullyGetBudget()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        List<BudgetGroupEntity> budgetDatabaseReturn = [
            new(99, "name 1", 59, 77, 99, [new(24, "Purposefully not equal", -21, 42, 56)]),
            new(23, "group name 2", 189, 154, 59, [new(65, "something fun", 121, 46, 32), new(10, "", 68, 108, 27)]),
        ];
        List<BudgetGroupResponse> expected = [
            new(99, "name 1", 59, 77, 99, [new(24, "Purposefully not equal", -21, 42, 56)]),
            new(23, "group name 2", 189, 154, 59, [new(65, "something fun", 121, 46, 32), new(10, "", 68, 108, 27)]),
        ];

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .ReturnsAsync(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object));

        _mockBudgetDatabase.Setup(x => x.GetBudget(authedUser)).Returns(Task.FromResult(budgetDatabaseReturn));

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _budgetService.GetBudget(tokenToDecode));

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBudgetDatabase.Verify(x => x.GetBudget(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
