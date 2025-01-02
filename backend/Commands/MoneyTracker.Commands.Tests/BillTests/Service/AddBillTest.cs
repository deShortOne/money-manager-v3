using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Common.Utilities.DateTimeUtil;
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

    private readonly int _payee = 311;
    private readonly decimal _amount = 75.24m;
    private readonly DateOnly _nextDueDate = new DateOnly(2024, 1, 24);
    private readonly string _frequency = "Weekly";
    private readonly int _category = 1;
    private readonly int _monthDay = 24;
    private readonly int _accountId = 2;
    private readonly NewBillRequest _newBillRequest;
    private readonly BillEntity _newBillEntity;

    public AddBillTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
        _newBillRequest = new NewBillRequest(_payee, _amount, _nextDueDate, _frequency, _category, _accountId);
        _newBillEntity = new BillEntity(_nextBillId, _payee, _amount, _nextDueDate, _monthDay, _frequency, _category, _accountId);
    }

    [Fact]
    public async void SuccessfullyAddNewBill()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _accountId))
            .Returns(Task.FromResult(true));

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
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _accountId), Times.Once);
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
    public void FailToAddNewBillDueToAccountNotBelongingToUser()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _accountId))
            .Returns(Task.FromResult(false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(_tokenToDecode, _newBillRequest);
            });
            Assert.Equal("Account not found", error.Message);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _accountId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToAddNewBillDueToInvalidFrequency()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _accountId))
            .Returns(Task.FromResult(true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(false);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(_tokenToDecode, _newBillRequest);
            });
            Assert.Equal("Invalid frequency", error.Message);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _accountId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void InvalidCategory_Fails()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _accountId))
            .Returns(Task.FromResult(true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(_frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(_category))
            .Returns(Task.FromResult(false));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(_prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(_prevBillId))
            .Returns(_nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(_nextDueDate)).Returns(_monthDay);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(_tokenToDecode, _newBillRequest);
            });
            Assert.Equal("Invalid category", error.Message);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _accountId), Times.AtMostOnce);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(_frequency), Times.AtMostOnce);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(_category), Times.AtMostOnce);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.AtMostOnce);
            _mockIdGenerator.Verify(x => x.NewInt(_prevBillId), Times.AtMostOnce);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
