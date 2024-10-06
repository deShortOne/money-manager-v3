
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Bill.Tests.Service;
public sealed class AddBillTest
{
    [Fact]
    public async void SuccessfullyAddNewBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var monthDay = 24;
        var accountId = 2;
        var newBillRequest = new NewBillRequestDTO(payee, amount, nextDueDate, frequency, category, monthDay, accountId);
        var newBillEntity = new NewBillDTO(payee, amount, nextDueDate, frequency, category, monthDay, accountId);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();
        mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(true));

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.AddBill(newBillEntity));

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object, new IdGenerator());

        await billService.AddBill(tokenToDecode, newBillRequest);

        Assert.Multiple(() =>
        {
            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            mockBillDatabase.Verify(x => x.AddBill(newBillEntity), Times.Once);
        });
    }
}
