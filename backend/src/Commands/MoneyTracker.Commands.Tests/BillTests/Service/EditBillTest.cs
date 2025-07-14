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
    public async Task SuccessfullyEditBill_OnlyChangeOneItem(int id, int? payeeId,
        decimal? amount, DateOnly? nextDueDate, int? monthDay, string frequency, int? category, int? payerId)
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var commonPayerId = 233;

        var editBillRequest = new EditBillRequest(id, payeeId, amount, nextDueDate, frequency, category, payerId);
        var editBillEntity = new EditBillEntity(id, payeeId, amount, nextDueDate, monthDay, frequency, category, payerId);

        _mockUserService
            .Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        if (payerId != null)
        {
            _mockAccountService
                .Setup(x => x.DoesUserOwnAccount(authedUser, (int)payerId, CancellationToken.None))
                .ReturnsAsync(true);
        }
        if (payeeId != null)
        {
            _mockAccountDatabase
                .Setup(x => x.GetAccountUserEntity((int)payeeId, CancellationToken.None))
                .ReturnsAsync(new AccountUserEntity((int)payeeId, 1, userId, false));
        }

        _mockBillDatabase
            .Setup(x => x.GetBillById(id, CancellationToken.None))
            .ReturnsAsync(new BillEntity(id, -1, -1, new DateOnly(), -1, "", -1, commonPayerId));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(commonPayerId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(commonPayerId, -1, userId, true));

        if (category != null)
        {
            _mockCategoryService.Setup(x => x.DoesCategoryExist((int)category, CancellationToken.None)).Returns(Task.FromResult(true));
        }

        if (frequency != null)
        {
            _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);
        }

        if (nextDueDate != null)
        {
            _mockMonthDayCalculator.Setup(x => x.Calculate((DateOnly)nextDueDate)).Returns(21); // constant!! :/
        }

        await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockBillDatabase.Verify(x => x.GetBillById(id, CancellationToken.None), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(commonPayerId, CancellationToken.None), Times.Once);

            if (payerId != null)
            {
                _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, (int)payerId, CancellationToken.None), Times.Once);
            }
            if (payeeId != null)
            {
                _mockAccountDatabase.Verify(x => x.GetAccountUserEntity((int)payeeId, CancellationToken.None), Times.Once);
            }
            if (category != null)
            {
                _mockCategoryService.Verify(x => x.DoesCategoryExist((int)category, CancellationToken.None), Times.Once);
            }

            if (frequency != null)
            {
                _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);
            }

            if (nextDueDate != null)
            {
                _mockMonthDayCalculator.Verify(x => x.Calculate((DateOnly)nextDueDate), Times.Once);
            }

            _mockBillDatabase.Verify(x => x.EditBill(editBillEntity, CancellationToken.None), Times.Once);

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

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockBillDatabase.Setup(x => x.GetBillById(billId, CancellationToken.None)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, payerId));
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(payerId, CancellationToken.None)).ReturnsAsync((AccountUserEntity)null);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Bill not found", result.Error.Description);

            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payerId, CancellationToken.None), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_PayerAccountDoesNotBelongToUser()
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
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None)).ReturnsAsync(false);

        _mockBillDatabase.Setup(x => x.GetBillById(billId, CancellationToken.None)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None)).ReturnsAsync(new AccountUserEntity(previousBillPayerId, previousBillPayerId, userId, true));

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", result.Error.Description);

            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToEditBill_PayeeAccountDoesNotBelongToUser()
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
        mockDateTime
            .Setup(x => x.Now)
            .Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserService
            .Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockAccountService
            .Setup(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None))
            .ReturnsAsync(true);

        _mockBillDatabase
            .Setup(x => x.GetBillById(billId, CancellationToken.None))
            .ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(previousBillPayerId, previousBillPayerId, userId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(payeeId, CancellationToken.None))
            .ReturnsAsync(new AccountUserEntity(previousBillPayerId, previousBillPayerId, userId + 1, true));

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", result.Error.Description);

            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None), Times.Once);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payeeId, CancellationToken.None), Times.Once);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);

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
        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Must have at least one non-null value", result.Error!.Description);

            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None), Times.Never);
            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.Once);

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

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None)).ReturnsAsync(true);

        _mockBillDatabase.Setup(x => x.GetBillById(billId, CancellationToken.None)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None)).ReturnsAsync(new AccountUserEntity(previousBillPayerId, -1, userId, true));
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(payeeId, CancellationToken.None)).ReturnsAsync(new AccountUserEntity(previousBillPayerId, -1, userId, true));

        _mockCategoryService.Setup(x => x.DoesCategoryExist(category, CancellationToken.None)).Returns(Task.FromResult(false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Category not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payeeId, CancellationToken.None), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(category, CancellationToken.None), Times.AtMostOnce);
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

        _mockUserService.Setup(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None))
            .ReturnsAsync(authedUser);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None)).ReturnsAsync(true);
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(payeeId, CancellationToken.None)).ReturnsAsync(new AccountUserEntity(35, 1, userId, false));

        _mockBillDatabase.Setup(x => x.GetBillById(billId, CancellationToken.None)).ReturnsAsync(new BillEntity(-1, -1, -1, new DateOnly(), -1, "", -1, previousBillPayerId));
        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None)).ReturnsAsync(new AccountUserEntity(previousBillPayerId, -1, userId, true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(false);

        var result = await _billService.EditBill(tokenToDecode, editBillRequest, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.Equal("Frequency type not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(tokenToDecode, CancellationToken.None), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(authedUser, payerId, CancellationToken.None), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(payeeId, CancellationToken.None), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(previousBillPayerId, CancellationToken.None), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetBillById(billId, CancellationToken.None), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
