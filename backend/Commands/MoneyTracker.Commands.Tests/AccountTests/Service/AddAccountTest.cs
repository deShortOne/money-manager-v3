using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Account;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.AccountTests.Service;
public class AddAccountTest : AccountTestHelper
{
    [Fact]
    public async Task SuccessfullyAddAccountThatBelongsToUser()
    {
        var token = "agdsa";
        var userId = 4;
        var accountId = 52;
        var doesAccountBelongToUser = true;

        _mockUserService
            .Setup(x => x.GetUserFromToken(token))
            .ReturnsAsync(new AuthenticatedUser(userId));

        await _accountService.AddAccount(token, new AddAccountToUserRequest(accountId, doesAccountBelongToUser));

        Assert.Multiple(() =>
        {
            _mockUserService
                .Verify(x => x.GetUserFromToken(token), Times.Once);

            _mockAccountDatabase
                .Verify(x => x.AddAccountToUser(new AccountUserEntity(accountId, userId, doesAccountBelongToUser)), Times.Once);

            _mockMessageBusClient
                .Verify(x => x.PublishEvent(new EventUpdate(new AuthenticatedUser(userId), DataTypes.Account), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task SuccessfullyAddAccountThatDoesntBelongsToUser()
    {
        var token = "agdsa";
        var userId = 4;
        var accountId = 52;
        var doesAccountBelongToUser = false;

        _mockUserService
            .Setup(x => x.GetUserFromToken(token))
            .ReturnsAsync(new AuthenticatedUser(userId));

        await _accountService.AddAccount(token, new AddAccountToUserRequest(accountId, doesAccountBelongToUser));

        Assert.Multiple(() =>
        {
            _mockUserService
                .Verify(x => x.GetUserFromToken(token), Times.Once);

            _mockAccountDatabase
                .Verify(x => x.AddAccountToUser(new AccountUserEntity(accountId, userId, doesAccountBelongToUser)), Times.Once);

            _mockMessageBusClient
                .Verify(x => x.PublishEvent(new EventUpdate(new AuthenticatedUser(userId), DataTypes.Account), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsToAddAccountAsTokenIsInvalid()
    {
        var token = "agdsa";
        var accountId = 52;
        var doesAccountBelongToUser = false;

        _mockUserService
            .Setup(x => x.GetUserFromToken(token))
            .ReturnsAsync(Error.Validation("123", "Invalid token"));

        await _accountService.AddAccount(token, new AddAccountToUserRequest(accountId, doesAccountBelongToUser));

        Assert.Multiple(() =>
        {
            _mockUserService
                .Verify(x => x.GetUserFromToken(token), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
