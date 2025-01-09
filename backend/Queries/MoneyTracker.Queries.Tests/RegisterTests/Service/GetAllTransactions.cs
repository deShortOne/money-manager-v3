using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Utilities.DateTimeUtil;
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
            new(1, 89, "Payee A", 120.50M, new DateOnly(2023, 11, 13), 80, "Category A", 69, "Account 1"),
            new(3, 78, "Payee B", 70.50M, new DateOnly(2024, 7, 25), 81, "Category 2", 66, "Account B"),
        ];
        List<TransactionResponse> expected = [
            new(1, new(89, "Payee A"), 120.50M, new DateOnly(2023, 11, 13), new (80, "Category A"), new(69, "Account 1")),
            new(3, new(78, "Payee B"), 70.50M, new DateOnly(2024, 7, 25), new(81, "Category 2"), new(66, "Account B")),
        ];

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .ReturnsAsync(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object));

        _mockRegisterDatabase.Setup(x => x.GetAllTransactions(authedUser)).Returns(Task.FromResult(budgetDatabaseReturn));

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _budgetService.GetAllTransactions(tokenToDecode));

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetAllTransactions(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
