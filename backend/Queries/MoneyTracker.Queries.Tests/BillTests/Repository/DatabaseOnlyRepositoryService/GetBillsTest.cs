using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.DatabaseOnlyRepositoryService;
public class GetAllBillsTest : DatabaseOnlyTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task CallOffToDatabaseOnce()
    {
        var bills = new List<BillEntity>
        {
            new(8, 4, "ahbd", 53, new DateOnly(), 2, "sdagg", 4, "Asd", 321, "pnfwb"),
            new(319, 563, "QYYbCMbZsu", 183, new DateOnly(), 2, "CJMkFSlBok", 272, "yAKoErAMiK", 927, "KIbcSMciyY"),
            new(890, 124, "bqNyVwFbmt", 106, new DateOnly(), 53, "YBvVPJhtkQ", 413, "kxvILdiuVv", 688, "eeQpHYFTxE")
        };
        _mockBillDatabase.Setup(x => x.GetAllBills(_authedUser))
            .ReturnsAsync(bills);

        await _billRepositoryService.GetAllBills(_authedUser);

        _mockBillDatabase.Verify(x => x.GetAllBills(_authedUser), Times.Once);
        VerifyNoOtherCalls();
    }
}
