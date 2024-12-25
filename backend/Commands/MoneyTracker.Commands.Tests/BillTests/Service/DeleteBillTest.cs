using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class DeleteBillTest : BillTestHelper
{
    [Fact]
    public async void SuccessfullyDeleteBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var deleteBillRequest = new DeleteBillRequest(billId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));
        _mockBillDatabase.Setup(x => x.DeleteBill(billId));

        await _billService.DeleteBill(tokenToDecode, deleteBillRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockBillDatabase.Verify(x => x.DeleteBill(billId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void BillDoesNotBelongToUser_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var editBillRequest = new DeleteBillRequest(billId);

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
                await _billService.DeleteBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
