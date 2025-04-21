using MoneyTracker.Commands.Domain.Entities.Account;
using Moq;

namespace MoneyTracker.Commands.Tests.AccountTests.Service;
public sealed class DoesUserOwnAccountTest : AccountTestHelper
{
    [Fact]
    public async Task UserDoesOwnAccount()
    {
        var accountId = 2;
        var userId = 4;
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(accountId))
            .ReturnsAsync(new AccountUserEntity(accountId, 35, userId, true));

        Assert.True(await _accountService.DoesUserOwnAccount(new(userId), accountId));
    }

    [Fact]
    public async Task UserDoesNotOwnAccount()
    {
        var accountId = 2;
        var userId1 = 4;
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(accountId))
            .ReturnsAsync(new AccountUserEntity(accountId, 35, userId1, false));

        Assert.False(await _accountService.DoesUserOwnAccount(new(userId1), accountId));
    }

    [Fact]
    public async Task AccountDoesNotExist()
    {
        var accountId = 2;
        var userId = 4;
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(accountId))
            .ReturnsAsync((AccountUserEntity)null);

        Assert.False(await _accountService.DoesUserOwnAccount(new(userId), accountId));
    }
}
