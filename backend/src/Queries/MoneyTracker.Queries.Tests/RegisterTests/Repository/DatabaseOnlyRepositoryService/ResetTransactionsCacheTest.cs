using MoneyTracker.Authentication.DTOs;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public class ResetTransactionsCacheTest : DatabaseOnlyTestHelper
{
    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await Assert.ThrowsAsync<NotImplementedException>(()
            => _registerRepositoryService.ResetTransactionsCache(It.IsAny<AuthenticatedUser>(), CancellationToken.None));
    }
}
