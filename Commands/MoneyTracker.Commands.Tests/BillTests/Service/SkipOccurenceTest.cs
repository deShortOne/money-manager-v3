using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Bill;
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));
        _mockBillDatabase.Setup(x => x.GetBillById(billId))
            .Returns(Task.FromResult(new BillEntity(userId, "", 0, new DateOnly(), monthDay, frequencyToCheck, -1, -1)));
        _mockBillDatabase.Setup(x => x.EditBill(editBillEntity));

        _mockFrequencyCalculation.Setup(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate))
            .Returns(dateToBecome);

        await _billService.SkipOccurence(tokenToDecode, skipBillOccurence);
        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.SkipOccurence(tokenToDecode, skipBillOccurence);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
