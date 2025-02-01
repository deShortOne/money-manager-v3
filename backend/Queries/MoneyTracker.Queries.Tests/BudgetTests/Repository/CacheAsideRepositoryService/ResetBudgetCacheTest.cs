using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.CacheAsideRepositoryService;
public class ResetBudgetCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        var budget = new List<BudgetGroupEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockBudgetDatabase.Setup(x => x.GetBudget(_authedUser))
            .ReturnsAsync(budget);

        await _budgetRepositoryService.ResetBudgetCache(_authedUser);

        _mockBudgetDatabase.Verify(x => x.GetBudget(_authedUser));
        _mockBudgetCache.Verify(x => x.SaveBudget(_authedUser, budget));
        VerifyNoOtherCalls();
    }
}
