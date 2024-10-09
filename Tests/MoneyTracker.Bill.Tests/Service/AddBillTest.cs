
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
public sealed class AddBillTest
{
    private readonly Mock<IDateProvider> _mockDateProvider = new();
    private readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();
    private readonly Mock<IAccountDatabase> _mockAccountDatabase = new();
    private readonly Mock<IBillDatabase> _mockBillDatabase = new();
    private readonly Mock<IIdGenerator> _mockIdGenerator = new();
    private readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();
    private readonly Mock<IMonthDayCalculator> _mockMonthDayCalculator = new();
    private readonly Mock<ICategoryDatabase> _mockCategoryDatabase = new();

    private BillService _billService;

    public AddBillTest()
    {
        _billService = new BillService(_mockBillDatabase.Object,
            _mockDateProvider.Object,
            _mockUserAuthService.Object,
            _mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockFrequencyCalculation.Object,
            _mockMonthDayCalculator.Object,
            _mockCategoryDatabase.Object);
    }

    private void EnsureAllMocksHadNoOtherCalls()
    {
        _mockDateProvider.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
        _mockMonthDayCalculator.VerifyNoOtherCalls();
        _mockCategoryDatabase.VerifyNoOtherCalls();
    }

    [Fact]
    public async void SuccessfullyAddNewBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        int prevBillId = 7;
        int nextBillId = 14;

        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Weekly";
        var category = 1;
        var monthDay = 24;
        var accountId = 2;
        var newBillRequest = new NewBillRequestDTO(payee, amount, nextDueDate, frequency, category, accountId);
        var newBillEntity = new NewBillEntity(nextBillId, payee, amount, nextDueDate, frequency, category, monthDay, accountId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode))
            .Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId))
            .Returns(Task.FromResult(true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(category))
            .Returns(Task.FromResult(true));

        _mockBillDatabase.Setup(x => x.GetLastId())
            .Returns(Task.FromResult(prevBillId));

        _mockIdGenerator.Setup(x => x.NewInt(prevBillId))
            .Returns(nextBillId);

        _mockMonthDayCalculator.Setup(x => x.Calculate(nextDueDate)).Returns(monthDay);

        _mockBillDatabase.Setup(x => x.AddBill(newBillEntity));

        await _billService.AddBill(tokenToDecode, newBillRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(category), Times.Once);
            _mockBillDatabase.Verify(x => x.GetLastId(), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(prevBillId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);
            _mockMonthDayCalculator.Verify(x => x.Calculate(nextDueDate), Times.Once);
            _mockBillDatabase.Verify(x => x.AddBill(newBillEntity), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToAddNewBillDueToAccountNotBelongingToUser()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "Monthly";
        var category = 1;
        var accountId = 2;
        var newBillRequest = new NewBillRequestDTO(payee, amount, nextDueDate, frequency, category, accountId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(false));


        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(tokenToDecode, newBillRequest);
            });
            Assert.Equal("Account not found", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void FailToAddNewBillDueToInvalidFrequency()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "One day at a time";
        var category = 1;
        var accountId = 2;
        var newBillRequest = new NewBillRequestDTO(payee, amount, nextDueDate, frequency, category, accountId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(true));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(false);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(tokenToDecode, newBillRequest);
            });
            Assert.Equal("Invalid frequency", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void InvalidCategory_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";

        var payee = "bree";
        var amount = 75.24m;
        var nextDueDate = new DateOnly(2024, 1, 24);
        var frequency = "BiWeekly";
        var category = 1;
        var accountId = 2;
        var newBillRequest = new NewBillRequestDTO(payee, amount, nextDueDate, frequency, category, accountId);

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(authedUser, accountId)).Returns(Task.FromResult(true));

        _mockCategoryDatabase.Setup(x => x.DoesCategoryExist(category)).Returns(Task.FromResult(false));

        _mockFrequencyCalculation.Setup(x => x.DoesFrequencyExist(frequency)).Returns(true);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await _billService.AddBill(tokenToDecode, newBillRequest);
            });
            Assert.Equal("Invalid category", error.Message);

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(authedUser, accountId), Times.Once);
            _mockCategoryDatabase.Verify(x => x.DoesCategoryExist(category), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.DoesFrequencyExist(frequency), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
