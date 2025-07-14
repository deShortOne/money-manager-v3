using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
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
            new(1, new(89, "Payee A"), 120.50M, new DateOnly(2023, 11, 13), new(80, "Category A"), new(69, "Account 1")),
            new(3, new(78, "Payee B"), 70.50M, new DateOnly(2024, 7, 25), new(81, "Category 2"), new(66, "Account B")),
        ];

        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.CheckValidation()).Returns(Result.Success());
        mockUserAuth.Setup(x => x.User).Returns(new UserEntity(userId, "", ""));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(mockUserAuth.Object);

        _mockRegisterDatabase.Setup(x => x.GetAllTransactions(authedUser, CancellationToken.None)).ReturnsAsync(budgetDatabaseReturn);

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _registerService.GetAllTransactions(tokenToDecode, CancellationToken.None));

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetAllTransactions(authedUser, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
