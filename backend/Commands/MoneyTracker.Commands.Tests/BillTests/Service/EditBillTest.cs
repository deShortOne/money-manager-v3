using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Bill;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class EditBillTest : BillTestHelper
{

    public static TheoryData<int, string, decimal?, DateOnly?, int?, string, int?, int?> OnlyOneItemNotNull = new() {
        { 1, "something funky here", null, null, null, null, null, null },
        { 2, null, 245.23m, null, null, null, null, null },
        { 3, null, null, new DateOnly(2023, 2, 21), 21, null, null, null }, // When date is updated, monthday is auto updated
        { 4, null, null, null, null, "Daily", null, null },
        { 5, null, null, null, null, null, 5, null },
        { 6, null, null, null, null, null, null, 2 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void SuccessfullyEditBill_OnlyChangeOneItem(int id, string payee,
        decimal? amount, DateOnly? nextDueDate, int? monthDay, string frequency, int? category, int? accountId)
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var editBillRequest = new EditBillRequest(id, payee, amount, nextDueDate, frequency, category, accountId);
        var editBillEntity = new EditBillEntity(id, payee, amount, nextDueDate, monthDay, frequency, category, accountId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

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

        if (nextDueDate != null)
        {
            _mockMonthDayCalculator.Setup(x => x.Calculate((DateOnly)nextDueDate)).Returns(21); // constant!! :/
        }

        await _billService.EditBill(tokenToDecode, editBillRequest);

        Assert.Multiple(() =>
        {
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, id), Times.Once);
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);

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

            if (nextDueDate != null)
            {
                _mockMonthDayCalculator.Verify(x => x.Calculate((DateOnly)nextDueDate), Times.Once);
            }
            else
            {
                _mockMonthDayCalculator.Verify(x => x.Calculate(It.IsAny<DateOnly>()), Times.Never);
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
        var editBillRequest = new EditBillRequest(billId, payee, amount, nextDueDate, frequency, category, accountId);

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
                await _billService.EditBill(tokenToDecode, editBillRequest);
            });
            Assert.Equal("Bill not found", error.Message);

            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);

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
        var editBillRequest = new EditBillRequest(billId, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

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
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);

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
        var editBillRequest = new EditBillRequest(billId, null, null, null, null, null, null);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

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
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);

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
        var editBillRequest = new EditBillRequest(billId, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

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

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
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
        var editBillRequest = new EditBillRequest(billId, payee, amount, nextDueDate, frequency, category, accountId);

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

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

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
