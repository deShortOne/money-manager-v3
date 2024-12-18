using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Entities.Bill;
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccounts(authedUser)).Returns(Task.FromResult(billDatabaseReturn));


        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _accountService.GetAccounts(tokenToDecode));

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccounts(authedUser), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
