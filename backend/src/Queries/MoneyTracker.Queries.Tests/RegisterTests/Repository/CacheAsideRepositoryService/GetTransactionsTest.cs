using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService;
public class GetAllTransactionsTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataInCacheWontCallOffToDatabase()
    {
        _mockRegisterCache.Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(new List<TransactionEntity>());

        await _registerRepositoryService.GetAllTransactions(_authedUser, CancellationToken.None);

        _mockRegisterCache.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None));
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var Transactions = new List<TransactionEntity>
        {
            new(697, 294, "CrwXioVANj", 392, new DateOnly(), 464, "KZjDxyyWxn", 663, "HuBKLFZLzw"),
            new(341, 254, "ZXrYRIZjAH", 946, new DateOnly(), 29, "oUFYOolNTI", 169, "EVOwvBPhGp"),
            new(66, 290, "mQuxYVliBr", 917, new DateOnly(), 253, "NzwRYulZfN", 429, "vPRBXvFuOc"),
        };

        _mockRegisterCache.Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));
        _mockRegisterDatabase.Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(Transactions);

        await _registerRepositoryService.GetAllTransactions(_authedUser, CancellationToken.None);

        _mockRegisterCache.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None));
        _mockRegisterDatabase.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None));
        _mockRegisterCache.Verify(x => x.SaveTransactions(_authedUser, Transactions));
        VerifyNoOtherCalls();
    }
}
