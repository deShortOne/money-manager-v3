
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService;
public class ResetTransactionsCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        var Transactions = new List<TransactionEntity>
        {
            new(384, 216, "OdAJwLOSzS", 995, new DateOnly(), 455, "MZoYhVDQmI", 937, "nhjrYkxvhJ"),
            new(183, 115, "OQEdRIfSIH", 871, new DateOnly(), 137, "qUzBrXTItq", 944, "NlPgKaONoW"),
            new(777, 64, "TWUhglzSSK", 127, new DateOnly(), 542, "PzNRgFliyc", 991, "eNMhdpxeDD"),
        };

        _mockRegisterDatabase.Setup(x => x.GetAllTransactions(_authedUser))
            .ReturnsAsync(Transactions);

        await _registerRepositoryService.ResetTransactionsCache(_authedUser);

        _mockRegisterDatabase.Verify(x => x.GetAllTransactions(_authedUser));
        _mockRegisterCache.Verify(x => x.SaveTransactions(_authedUser, Transactions));
        VerifyNoOtherCalls();
    }
}
