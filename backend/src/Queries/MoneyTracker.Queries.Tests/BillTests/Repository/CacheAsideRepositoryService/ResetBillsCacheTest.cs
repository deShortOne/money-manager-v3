using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.CacheAsideRepositoryService;
public class ResetBillsCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        var bills = new List<BillEntity>
        {
            new(666, 794, "YyOArEkHeh", 785, new DateOnly(), 151, "SsMxXABPRn", 127, "urxYJHIRVK", 391, "KwROPAJYJk"),
            new(716, 438, "fnQXAkpijE", 417, new DateOnly(), 73, "ZQtMiZUkMa", 17, "MXBiJrLmnh", 971, "KgUUJbluEI"),
            new(810, 249, "QOmkCiamDQ", 366, new DateOnly(), 908, "qzZMryEZDk", 881, "QiqUygOyjY", 624, "WPyLfUWtEI"),
        };

        _mockBillDatabase.Setup(x => x.GetAllBills(_authedUser, CancellationToken.None))
            .ReturnsAsync(bills);

        await _billRepositoryService.ResetBillsCache(_authedUser, CancellationToken.None);

        _mockBillDatabase.Verify(x => x.GetAllBills(_authedUser, CancellationToken.None));
        _mockBillCache.Verify(x => x.SaveBills(_authedUser, bills));
        VerifyNoOtherCalls();
    }
}
