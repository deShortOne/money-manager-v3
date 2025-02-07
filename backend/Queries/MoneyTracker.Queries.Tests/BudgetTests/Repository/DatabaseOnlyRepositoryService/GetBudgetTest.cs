using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.DatabaseOnlyRepositoryService;
public class GetBudgetTest : DatabaseOnlyTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var budgets = new List<BudgetGroupEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };
        _mockBudgetDatabase.Setup(x => x.GetBudget(_authedUser))
            .ReturnsAsync(budgets);

        await _budgetRepositoryService.GetBudget(_authedUser);

        _mockBudgetDatabase.Verify(x => x.GetBudget(_authedUser), Times.Once);
        VerifyNoOtherCalls();
    }
}
