using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.CacheAsideRepositoryService;
public class GetAllBillsTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataInCacheWontCallOffToDatabase()
    {
        _mockBillCache.Setup(x => x.GetAllBills(_authedUser, CancellationToken.None))
            .ReturnsAsync(new List<BillEntity>());

        await _billRepositoryService.GetAllBills(_authedUser, CancellationToken.None);

        _mockBillCache.Verify(x => x.GetAllBills(_authedUser, CancellationToken.None));
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var Bills = new List<BillEntity>
        {
            new(8, 4, "ahbd", 53, new DateOnly(), 2, "sdagg", 4, "Asd", 321, "pnfwb"),
            new(319, 563, "QYYbCMbZsu", 183, new DateOnly(), 2, "CJMkFSlBok", 272, "yAKoErAMiK", 927, "KIbcSMciyY"),
            new(890, 124, "bqNyVwFbmt", 106, new DateOnly(), 53, "YBvVPJhtkQ", 413, "kxvILdiuVv", 688, "eeQpHYFTxE")
        };

        _mockBillCache.Setup(x => x.GetAllBills(_authedUser, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));
        _mockBillDatabase.Setup(x => x.GetAllBills(_authedUser, CancellationToken.None))
            .ReturnsAsync(Bills);

        await _billRepositoryService.GetAllBills(_authedUser, CancellationToken.None);

        _mockBillCache.Verify(x => x.GetAllBills(_authedUser, CancellationToken.None));
        _mockBillDatabase.Verify(x => x.GetAllBills(_authedUser, CancellationToken.None));
        _mockBillCache.Verify(x => x.SaveBills(_authedUser, Bills));
        VerifyNoOtherCalls();
    }
}
