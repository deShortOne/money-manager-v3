using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Result;
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
        var expected = ResultT<List<AccountResponse>>.Success([
            new(1, "fds"),
            new(2, "jgf"),
        ]);

        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.CheckValidation()).Returns(Result.Success());
        mockUserAuth.Setup(x => x.User).Returns(new UserEntity(userId, "", ""));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(mockUserAuth.Object);

        _mockAccountDatabase.Setup(x => x.GetAccountsOwnedByUser(authedUser, CancellationToken.None)).ReturnsAsync(billDatabaseReturn);

        Assert.Multiple(async () =>
        {
            var accounts = await _accountService.GetAccounts(tokenToDecode, CancellationToken.None);
            //Assert.Equal(expected.Value, accounts.Value);

            Assert.Equal(expected, accounts);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountsOwnedByUser(authedUser, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
