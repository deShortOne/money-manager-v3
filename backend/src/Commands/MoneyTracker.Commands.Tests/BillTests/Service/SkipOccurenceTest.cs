using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class SkipOccurenceTest : BillTestHelper
{
    [Fact]
    public async Task SuccessfullySkipOccurenceInBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var dateToEvaluate = new DateOnly(2024, 10, 8);
        var dateToBecome = new DateOnly(2024, 11, 8);
        var skipBillOccurence = new SkipBillOccurrenceRequest(billId, dateToEvaluate);
        var frequencyToCheck = "POLEMNB";
        var monthDay = -2;
        var editBillEntity = new EditBillEntity(billId, nextDueDate: dateToBecome);
        var previousBillPayerId = 1734;

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockBillDatabase
            .Setup(x => x.GetBillById(billId, CancellationToken.None))
            .Returns(Task.FromResult(new BillEntity(billId, 0, 0, new DateOnly(), monthDay, frequencyToCheck, -1, previousBillPayerId)));

        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(previousBillPayerId, -1, userId, true));

        _mockFrequencyCalculation.Setup(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate))
            .Returns(dateToBecome);

        var result = await _billService.SkipOccurence(tokenToDecode, skipBillOccurence, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.True(result.IsSuccess);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.EditBill(editBillEntity, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Bill), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task BillDoesntBelongToUser_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var dateToEvaluate = new DateOnly(2024, 10, 8);
        var dateToBecome = new DateOnly(2024, 11, 8);
        var skipBillOccurence = new SkipBillOccurrenceRequest(billId, dateToEvaluate);
        var previousBillPayerId = 782;

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService
            .Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockBillDatabase
            .Setup(x => x.GetBillById(billId, CancellationToken.None))
            .Returns(Task.FromResult(new BillEntity(-1, 0, 0, new DateOnly(), -1, "", -1, previousBillPayerId)));

        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(35, billId, userId, false));

        var result = await _billService.SkipOccurence(tokenToDecode, skipBillOccurence, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Bill not found", result.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
