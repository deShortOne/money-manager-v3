
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyTracker.Calculation.Bill;
using MoneyTracker.Calculation.Bill.Frequencies;
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
public sealed class EditBillTest
{

    public static TheoryData<int, string?, decimal?, DateOnly?, string?, int?, int?> OnlyOneItemNotNull = new() {
        { 1, "something funky here", null, null, null, null, null },
        { 2, null, 245.23m, null, null, null, null },
        { 3, null, null, new DateOnly(2023, 2, 21), null, null, null },
        { 4, null, null, null, "frequency goes here", null, null },
        { 5, null, null, null, null, 5, null },
        { 6, null, null, null, null, null, 2 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void SuccessfullyEditBill_OnlyChangeOneItem(int id, string? payee,
        decimal? amount, DateOnly? nextDueDate, string? frequency, int? category, int? accountId)
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var editBillRequest = new EditBillRequestDTO(id, payee, amount, nextDueDate, frequency, category, accountId);
        var editBillEntity = new EditBillEntity(id, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();
        if (accountId != null)
        {
            mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, (int)accountId)).Returns(Task.FromResult(true));
        }

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, id)).Returns(Task.FromResult(true));
        mockBillDatabase.Setup(x => x.EditBill(editBillEntity));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object,
            mockIdGenerator.Object,
            new FrequencyCalculation(),
            new MonthDayCalculator());

        await billService.EditBill(tokenToDecode, editBillRequest);

        Assert.Multiple(() =>
        {
            mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, id), Times.Once);
            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);

            if (accountId != null)
            {
                mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, (int)accountId), Times.Once);
            }
            else
            {
                mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(It.IsAny<AuthenticatedUser>(), It.IsAny<int>()), Times.Never);
            }

            mockBillDatabase.Verify(x => x.EditBill(editBillEntity), Times.Once);
        });
    }

    [Fact]
    public void FailToEditBill_BillDoesNotBelongToUser()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var accountId = 2;
        var editBillRequest = new EditBillRequestDTO(billId, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();


        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object,
            mockIdGenerator.Object,
            new FrequencyCalculation(),
            new MonthDayCalculator());


        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Bill not found", error.Message);

            mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
        });
    }

    [Fact]
    public void FailToEditBill_AccountDoesNotBelongToUser()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var accountId = 2;
        var editBillRequest = new EditBillRequestDTO(billId, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();
        mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(false));

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object,
            mockIdGenerator.Object,
            new FrequencyCalculation(),
            new MonthDayCalculator());


        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Account not found", error.Message);

            mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
        });
    }
}
