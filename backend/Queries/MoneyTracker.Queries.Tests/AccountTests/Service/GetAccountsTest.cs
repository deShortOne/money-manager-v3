using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Queries.Domain.Entities.Account;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Service;
public sealed class GetAccountsTest : AccountTestHelper
{
    [Fact]
    public void SuccessfullyGetBills()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var secondResponseOverdueBillInfo = new OverDueBillInfo(5, []);
        List<AccountEntity> billDatabaseReturn = [
            new(1, "fds"),
            new(2, "jgf"),
        ];
        List<AccountResponse> expected = [
            new(1, "fds"),
            new(2, "jgf"),
        ];


        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .ReturnsAsync(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object));

        _mockAccountDatabase.Setup(x => x.GetAccounts(authedUser)).Returns(Task.FromResult(billDatabaseReturn));


        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _accountService.GetAccounts(tokenToDecode));

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccounts(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
