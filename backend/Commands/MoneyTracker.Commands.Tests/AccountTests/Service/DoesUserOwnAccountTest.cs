using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using Moq;

namespace MoneyTracker.Commands.Tests.AccountTests.Service;
public sealed class DoesUserOwnAccountTest
{
    private Mock<IAccountCommandRepository> _mockAccountDb;
    private AccountService _accountService;

    public DoesUserOwnAccountTest()
    {
        _mockAccountDb = new Mock<IAccountCommandRepository>();
        _accountService = new AccountService(_mockAccountDb.Object);
    }

    [Fact]
    public async Task UserDoesOwnAccount()
    {
        var accountId = 2;
        var userId = 4;
        _mockAccountDb.Setup(x => x.GetAccountUserEntity(accountId, userId))
            .ReturnsAsync(new AccountUserEntity(accountId, userId, true));

        Assert.True(await _accountService.DoesUserOwnAccount(new(userId), accountId));
    }

    [Fact]
    public async Task UserDoesNotOwnAccount()
    {
        var accountId = 2;
        var userId1 = 4;
        _mockAccountDb.Setup(x => x.GetAccountUserEntity(accountId, 1))
            .ReturnsAsync(new AccountUserEntity(accountId, userId1, false));

        Assert.False(await _accountService.DoesUserOwnAccount(new(userId1), accountId));
    }

    [Fact]
    public async Task AccountDoesNotExist()
    {
        var accountId = 2;
        var userId = 4;
        _mockAccountDb.Setup(x => x.GetAccountUserEntity(accountId, 1))
            .ReturnsAsync((AccountUserEntity)null);

        Assert.False(await _accountService.DoesUserOwnAccount(new(userId), accountId));
    }
}
