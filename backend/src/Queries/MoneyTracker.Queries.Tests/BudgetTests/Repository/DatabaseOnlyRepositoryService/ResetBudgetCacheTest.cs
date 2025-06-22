using MoneyTracker.Authentication.DTOs;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.DatabaseOnlyRepositoryService;
public class ResetBudgetCacheTest : DatabaseOnlyTestHelper
{
    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await Assert.ThrowsAsync<NotImplementedException>(()
            => _budgetRepositoryService.ResetBudgetCache(It.IsAny<AuthenticatedUser>()));
    }
}
