
using System.Collections.Generic;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService.GivenGetAllTransactionRequest;
public class WhenDataIsNotInCache : CacheAsideTestHelper
{
    private AuthenticatedUser _authedUser = new(36);
    private List<TransactionEntity> _transactions;

    private List<TransactionEntity> _resultTransactions;

    public override async Task InitializeAsync()
    {
        _mockRegisterCache
            .Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));


        _transactions = new List<TransactionEntity>
        {
            new(697, 294, "CrwXioVANj", 392, new DateOnly(), 464, "KZjDxyyWxn", 663, "HuBKLFZLzw"),
            new(341, 254, "ZXrYRIZjAH", 946, new DateOnly(), 29, "oUFYOolNTI", 169, "EVOwvBPhGp"),
            new(66, 290, "mQuxYVliBr", 917, new DateOnly(), 253, "NzwRYulZfN", 429, "vPRBXvFuOc"),
        };
        _mockRegisterDatabase
            .Setup(x => x.GetAllTransactions(_authedUser, CancellationToken.None))
            .ReturnsAsync(_transactions);
        _mockRegisterCache
            .Setup(x => x.SaveTransactions(_authedUser, It.IsAny<List<TransactionEntity>>(), CancellationToken.None))
            .Callback((AuthenticatedUser _, List<TransactionEntity> transactions, CancellationToken _) => _resultTransactions = transactions);

        await _registerRepositoryService.GetAllTransactions(_authedUser, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenItWillCallOffToTheCacheOnce()
    {
        _mockRegisterCache.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenItWillCallOffToTheDatabaseOnce()
    {
        _mockRegisterDatabase.Verify(x => x.GetAllTransactions(_authedUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenItWillSaveIntoTheCacheCorrectly()
    {
        Assert.Equal(_transactions, _resultTransactions);
    }
}
