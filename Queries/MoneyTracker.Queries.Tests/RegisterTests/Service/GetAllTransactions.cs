using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Contracts.Responses.Transaction;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service;
public sealed class GetAllTransactionsTest : RegisterTestHelper
{
    [Fact]
    public void SuccessfullyGetBBudget()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        List<TransactionEntity> budgetDatabaseReturn = [
            new(1, "Payee A", 120.50M, new DateOnly(2023, 11, 13), "Category A", "Account 1"),
            new(3, "Payee B", 70.50M, new DateOnly(2024, 7, 25), "Category 2", "Account B"),
        ];
        List<TransactionResponse> expected = [
            new(1, "Payee A", 120.50M, new DateOnly(2023, 11, 13), "Category A", "Account 1"),
            new(3, "Payee B", 70.50M, new DateOnly(2024, 7, 25), "Category 2", "Account B"),
        ];

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockRegisterDatabase.Setup(x => x.GetAllTransactions(authedUser)).Returns(Task.FromResult(budgetDatabaseReturn));

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _budgetService.GetAllTransactions(tokenToDecode));

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetAllTransactions(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
