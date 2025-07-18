using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class
    DeleteBillTest : BillTestHelper
{
    [Fact]
    public async Task SuccessfullyDeleteBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var deleteBillRequest = new DeleteBillRequest(billId);
        var payerId = 23;

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockBillDatabase
            .Setup(x => x.GetBillById(billId, CancellationToken.None))
            .ReturnsAsync(new BillEntity(billId, -1, -1, new DateOnly(), -1, "", 1, payerId));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(payerId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(payerId, 1, userId, true));

        await _billService.DeleteBill(tokenToDecode, deleteBillRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.DeleteBill(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payerId, CancellationToken.None), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Bill), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task BillDoesNotBelongToUser_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var editBillRequest = new DeleteBillRequest(billId);
        var payerId = 17;

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService
            .Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockBillDatabase
            .Setup(x => x.GetBillById(billId, CancellationToken.None))
            .ReturnsAsync(new BillEntity(billId, -1, -1, new DateOnly(), -1, "", 1, payerId));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(payerId, CancellationToken.None))
            .ReturnsAsync((AccountUserEntity)null);

        var result = await _billService.DeleteBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Bill not found", result.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payerId, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
