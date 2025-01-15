using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Bill;
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
    public async void SuccessfullyAddNewBill()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payeeId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        _mockBillDatabase.Setup(x => x.AddBill(_newBillEntity));

        await _billService.AddBill(_tokenToDecode, _newBillRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payeeId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.Once);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.Once);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.Once);
            _mockMonthDayCalculator.Verify(x => x.Calculate(_nextDueDate), Times.Once);
            _mockBillDatabase.Verify(x => x.AddBill(_newBillEntity), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToAddNewBillDueToAccountNotBelongingToUser()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId))
            .ReturnsAsync(new AccountEntity(1, "", _userId + 1));
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payeeId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToAddNewBillDueToInvalidFrequency()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payeeId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(false);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Frequency type not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task InvalidCategory_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payeeId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(false));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(async () =>
        {
            Assert.Equal("Invalid category", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payeeId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task InvalidPayeeAccount_Fails()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(ResultT<AuthenticatedUser>.Success(_authedUser));

        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId))
            .ReturnsAsync(new AccountEntity(1, "", _userId));
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payeeId))
            .ReturnsAsync((AccountEntity)null);

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        var result = await _billService.AddBill(_tokenToDecode, _newBillRequest);
        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", result.Error!.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.AtMostOnce);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payeeId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

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
}
