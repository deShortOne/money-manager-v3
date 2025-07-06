using MoneyTracker.Authentication.DTOs;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public class ResetTransactionsCacheTest : DatabaseOnlyTestHelper
{
    public override Task InitializeAsync() => Task.CompletedTask;

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ThenTheErrorMessageIsCorrect()
    {
        await Assert.ThrowsAsync<NotImplementedException>(()
            => _registerRepositoryService.ResetTransactionsCache(It.IsAny<AuthenticatedUser>(), CancellationToken.None));
    }
}
