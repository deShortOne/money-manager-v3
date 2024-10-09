
using MoneyTracker.Calculation.Bill;
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
public sealed class EditBillTest : BillTestHelper
{

    public static TheoryData<int, string?, decimal?, DateOnly?, string?, int?, int?> OnlyOneItemNotNull = new() {
        { 1, "something funky here", null, null, null, null, null },
        { 2, null, 245.23m, null, null, null, null },
        { 3, null, null, new DateOnly(2023, 2, 21), null, null, null },
        { 4, null, null, null, "Daily", null, null },
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        if (accountId != null)
        {
            _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, (int)accountId)).Returns(Task.FromResult(true));
        }

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, id)).Returns(Task.FromResult(true));
        _mockBillDatabase.Setup(x => x.EditBill(editBillEntity));

        if (category != null)
        {
            _mockCategoryDatabase.Setup(x => x.DoesCategoryExist((int)category)).Returns(Task.FromResult(true));
        }

        if (frequency != null)
        {
            _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);
        }

        await _billService.EditBill(tokenToDecode, editBillRequest);

        Assert.Multiple(() =>
        {
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, id), Times.Once);
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);

            if (accountId != null)
            {
                _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, (int)accountId), Times.Once);
            }
            else
            {
                _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(It.IsAny<AuthenticatedUser>(), It.IsAny<int>()), Times.Never);
            }
            if (category != null)
            {
                _mockCategoryDatabase.Verify(x => x.DoesCategoryExist((int)category), Times.Once);
            }
            else
            {
                _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(It.IsAny<int>()), Times.Never);
            }

            if (frequency != null)
            {
                _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);
            }
            else
            {
                _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(It.IsAny<string>()), Times.Never);
            }

            _mockBillDatabase.Verify(x => x.EditBill(editBillEntity), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));


        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(false));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Account not found", error.Message);

            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToEditBill_AllValuesInAreNull()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var editBillRequest = new EditBillRequestDTO(billId, null, null, null, null, null, null);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Must have at least one non-null value", error.Message);

            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(It.IsAny<AuthenticatedUser>(), It.IsAny<int>()), Times.Never);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(It.IsAny<AuthenticatedUser>(), It.IsAny<int>()), Times.Never);
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToEditBill_InvalidCategory()
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(category)).Returns(Task.FromResult(false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Invalid category", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(category), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToEditBill_InvalidFrequency()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "frequency goes here";
        var category = 1;
        var accountId = 2;
        var editBillRequest = new EditBillRequestDTO(billId, payee, amount, nextDueDate, frequency, category, accountId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(false);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Invalid frequency", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
