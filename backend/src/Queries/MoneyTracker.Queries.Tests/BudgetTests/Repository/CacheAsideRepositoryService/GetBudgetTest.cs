using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.CacheAsideRepositoryService;
public class GetBudgetTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataInCacheWontCallOffToDatabase()
    {
        _mockBudgetCache.Setup(x => x.GetBudget(_authedUser, CancellationToken.None))
            .ReturnsAsync(new List<BudgetGroupEntity>());

        await _budgetRepositoryService.GetBudget(_authedUser, CancellationToken.None);

        _mockBudgetCache.Verify(x => x.GetBudget(_authedUser, CancellationToken.None));
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var budgets = new List<BudgetGroupEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockBudgetCache.Setup(x => x.GetBudget(_authedUser, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));
        _mockBudgetDatabase.Setup(x => x.GetBudget(_authedUser, CancellationToken.None))
            .ReturnsAsync(budgets);

        await _budgetRepositoryService.GetBudget(_authedUser, CancellationToken.None);

        _mockBudgetCache.Verify(x => x.GetBudget(_authedUser, CancellationToken.None));
        _mockBudgetDatabase.Verify(x => x.GetBudget(_authedUser, CancellationToken.None));
        _mockBudgetCache.Verify(x => x.SaveBudget(_authedUser, budgets, CancellationToken.None));
        VerifyNoOtherCalls();
    }
}
