using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class EditBillTest : BillTestHelper
{

    public static TheoryData<int, int?, decimal?, DateOnly?, int?, string, int?, int?> OnlyOneItemNotNull = new() {
        { 1, 7, null, null, null, null, null, null },
        { 2, null, 245.23m, null, null, null, null, null },
        { 3, null, null, new DateOnly(2023, 2, 21), 21, null, null, null }, // When date is updated, monthday is auto updated
        { 4, null, null, null, null, "Daily", null, null },
        { 5, null, null, null, null, null, 5, null },
        { 6, null, null, null, null, null, null, 2 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void SuccessfullyEditBill_OnlyChangeOneItem(int id, int? payee,
        decimal? amount, DateOnly? nextDueDate, int? monthDay, string frequency, int? category, int? payerId)
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var commonPayerId = 233;

        var editBillRequest = new EditBillRequest(id, payee, amount, nextDueDate, frequency, category, payerId);
        var editBillEntity = new EditBillEntity(id, payee, amount, nextDueDate, monthDay, frequency, category, payerId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        if (payerId != null)
        {
            _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, (int)payerId))
                .ReturnsAsync(true);
        }
        if (payee != null)
        {
            _mockAccountDatabase.Setup(x => x.GetAccountById((int)payee))
                .ReturnsAsync(new AccountEntity(1, "", userId));
        }

        _mockBillDatabase.Setup(x => x.GetBillById(id)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, commonPayerId));
        _mockBillDatabase.Setup(x => x.EditBill(editBillEntity));
        _mockAccountDatabase.Setup(x => x.GetAccountById(commonPayerId)).ReturnsAsync(new AccountEntity(-1, "", userId));

        if (category != null)
        {
            _mockCategoryService.Setup(x => x.DoesCategoryExist((int)category)).Returns(Task.FromResult(true));
        }

        if (frequency != null)
        {
            _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);
        }

        if (nextDueDate != null)
        {
            _mockMonthDayCalculator.Setup(x => x.Calculate((DateOnly)nextDueDate)).Returns(21); // constant!! :/
        }

        await _billService.EditBill(tokenToDecode, editBillRequest);

        Assert.Multiple(() =>
        {
            _mockBillDatabase.Verify(x => x.GetBillById(id), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(commonPayerId), Times.Once);

            if (payerId != null)
            {
                _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, (int)payerId), Times.Once);
            }
            if (payee != null)
            {
                _mockAccountDatabase.Verify(x => x.GetAccountById((int)payee), Times.Once);
            }
            if (category != null)
            {
                _mockCategoryService.Verify(x => x.DoesCategoryExist((int)category), Times.Once);
            }

            if (frequency != null)
            {
                _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);
            }

            if (nextDueDate != null)
            {
                _mockMonthDayCalculator.Verify(x => x.Calculate((DateOnly)nextDueDate), Times.Once);
            }

            _mockBillDatabase.Verify(x => x.EditBill(editBillEntity), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(authedUser, DataTypes.Bill), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_BillDoesNotBelongToUser()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payeeId = 174;
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var payerId = 2;
        var editBillRequest = new EditBillRequest(billId, payeeId, amount, nextDueDate, frequency, category, payerId);

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        _mockBillDatabase.Setup(x => x.GetBillById(billId)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, payerId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(payerId)).ReturnsAsync((AccountEntity)null);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Bill not found", result.Error.Description);

            _mockBillDatabase.Verify(x => x.GetBillById(billId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(payerId), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_AccountDoesNotBelongToUser()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payeeId = 263;
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var payerId = 2;
        var previousBillPayerId = 623;
        var editBillRequest = new EditBillRequest(billId, payeeId, amount, nextDueDate, frequency, category, payerId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId)).ReturnsAsync(false);

        _mockBillDatabase.Setup(x => x.GetBillById(billId)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(previousBillPayerId)).ReturnsAsync(new AccountEntity(-1, "", userId));

        var result = await _billService.EditBill(tokenToDecode, editBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", result.Error.Description);

            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(previousBillPayerId), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_AllValuesInAreNull()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var editBillRequest = new EditBillRequest(billId, null, null, null, null, null, null);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Must have at least one non-null value", result.Error!.Description);

            _mockAccountDatabase.Verify(x => x.GetAccountById(It.IsAny<int>()), Times.Never);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_InvalidCategory()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payeeId = 236;
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var payerId = 2;
        var previousBillPayerId = 235;
        var editBillRequest = new EditBillRequest(billId, payeeId, amount, nextDueDate, frequency, category, payerId);

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId)).ReturnsAsync(true);
        _mockAccountDatabase.Setup(x => x.GetAccountById(payeeId)).ReturnsAsync(new AccountEntity(1, "", userId));

        _mockBillDatabase.Setup(x => x.GetBillById(billId)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(previousBillPayerId)).ReturnsAsync(new AccountEntity(-1, "", userId));

        _mockCategoryService.Setup(x => x.DoesCategoryExist(category)).Returns(Task.FromResult(false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Category not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(payeeId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(previousBillPayerId), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetBillById(billId), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(category), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_InvalidFrequency()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payeeId = 124;
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "frequency goes here";
        var category = 1;
        var payerId = 2;
        var previousBillPayerId = 8942;
        var editBillRequest = new EditBillRequest(billId, payeeId, amount, nextDueDate, frequency, category, payerId);

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId)).ReturnsAsync(true);
        _mockAccountDatabase.Setup(x => x.GetAccountById(payeeId)).ReturnsAsync(new AccountEntity(1, "", userId));

        _mockBillDatabase.Setup(x => x.GetBillById(billId)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(previousBillPayerId)).ReturnsAsync(new AccountEntity(-1, "", userId));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(false);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Frequency type not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(payeeId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(previousBillPayerId), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetBillById(billId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
