using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Bill;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public sealed class AddBillTest : BillTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _prevBillId = 7;
    private readonly int _nextBillId = 14;

    private readonly int _payeeId = 311;
    private readonly decimal _amount = 75.24m;
    private readonly DateOnly _nextDueDate = new DateOnly(2024, 1, 24);
    private readonly string _frequency = "Weekly";
    private readonly int _category = 1;
    private readonly int _monthDay = 24;
    private readonly int _payerId = 2;
    private readonly NewBillRequest _newBillRequest;
    private readonly BillEntity _newBillEntity;

    public AddBillTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
        _newBillRequest = new NewBillRequest(_payeeId, _amount, _nextDueDate, _frequency, _category, _payerId);
        _newBillEntity = new BillEntity(_nextBillId, _payeeId, _amount, _nextDueDate, _monthDay, _frequency, _category, _payerId);
    }

    [Fact]
    public async Task SuccessfullyAddNewBill()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId, false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockBillDatabase.Setup(x => x.AddBill(_newBillEntity));

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(true);

        await _billService.AddBill(_tokenToDecode, _newBillRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.Once);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.Once);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.Once);
            _mockMonthDayCalculator.Verify(x => x.Calculate(_nextDueDate), Times.Once);
            _mockBillDatabase.Verify(x => x.AddBill(_newBillEntity), Times.Once);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(_authedUser, DataTypes.Bill), It.IsAny<CancellationToken>()), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToAddNewBillDueToPayerAccountNotBelongingToUser()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId, false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(false);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToAddNewBillDueToInvalidFrequency()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId, false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(false);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(true);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Frequency type not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task InvalidCategory_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId, false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(false));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(true);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Category not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task InvalidPayeeAccount_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync((AccountUserEntity)null);

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(true);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task AmountIsNegative_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        var customBillRequest = new NewBillRequest(_payeeId, -_amount, _nextDueDate, _frequency, _category, _payerId);

        var result = await _billService.AddBill(_tokenToDecode, customBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Amount must be a positive number", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task ValidPayeeAccountButDoesNotBelongToUser_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId + 1, false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryService.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockBillDatabase.Setup(x => x.AddBill(_newBillEntity));

        _mockAccountService.Setup(x => x.DoesUserOwnAccount(_authedUser, _payerId))
            .ReturnsAsync(true);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", result.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockMonthDayCalculator.Verify(x => x.Calculate(_nextDueDate), Times.AtMostOnce);
            _mockAccountService.Verify(x => x.DoesUserOwnAccount(_authedUser, _payerId), Times.AtMostOnce);

            _mockBillDatabase.Verify(x => x.AddBill(_newBillEntity), Times.Never);

            _mockMessageBusClient.Verify(x => x.PublishEvent(new EventUpdate(_authedUser, DataTypes.Bill), It.IsAny<CancellationToken>()), Times.Never);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
