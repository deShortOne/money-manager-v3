
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Moq;

namespace MoneyTracker.Bill.Tests.Service;
public sealed class DeleteBillTest : BillTestHelper
{
    [Fact]
    public async void SuccessfullyDeleteBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var deleteBillRequest = new DeleteBillRequestDTO(billId);
        var deleteBillEntity = new DeleteBillDTO(billId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));
        _mockBillDatabase.Setup(x => x.DeleteBill(billId));

        await _billService.DeleteBill(tokenToDecode, deleteBillRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
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
        var editBillRequest = new DeleteBillRequestDTO(billId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.DeleteBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
