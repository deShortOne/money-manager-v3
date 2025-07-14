

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService.GivenGetAllTransactionRequest;
public class WhenDataIsInCache : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);
    public override async Task InitializeAsync()
    {
        _mockRegisterCache.Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(new List<TransactionEntity>());

        await _registerRepositoryService.GetAllTransactions(_authedUser, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenItWillCallOffToTheCacheOnce()
    {
        _mockRegisterCache.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenItWillNotCallOffToTheDatabase()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
    }
}
