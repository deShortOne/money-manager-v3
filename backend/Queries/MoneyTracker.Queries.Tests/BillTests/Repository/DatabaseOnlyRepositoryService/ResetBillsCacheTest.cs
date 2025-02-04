using MoneyTracker.Authentication.DTOs;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.DatabaseOnlyRepositoryService;
public class ResetBillsCacheTest : DatabaseOnlyTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await Assert.ThrowsAsync<NotImplementedException>(async ()
            => await _billRepositoryService.ResetBillsCache(It.IsAny<AuthenticatedUser>()));
    }
}
