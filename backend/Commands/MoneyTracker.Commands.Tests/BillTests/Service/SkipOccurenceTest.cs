using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class SkipOccurenceTest : BillTestHelper
{
    [Fact]
    public async void SuccessfullySkipOccurenceInBill()
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

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));
        _mockBillDatabase.Setup(x => x.GetBillById(billId))
            .Returns(Task.FromResult(new BillEntity(userId, "", 0, new DateOnly(), monthDay, frequencyToCheck, -1, -1)));
        _mockBillDatabase.Setup(x => x.EditBill(editBillEntity));

        _mockFrequencyCalculation.Setup(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate))
            .Returns(dateToBecome);

        await _billService.SkipOccurence(tokenToDecode, skipBillOccurence);
        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId), Times.Once);
            _mockBillDatabase.Verify(x => x.EditBill(editBillEntity), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void BillDoesntBelongToUser_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var dateToEvaluate = new DateOnly(2024, 10, 8);
        var dateToBecome = new DateOnly(2024, 11, 8);
        var skipBillOccurence = new SkipBillOccurrenceRequest(billId, dateToEvaluate);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.SkipOccurence(tokenToDecode, skipBillOccurence);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
