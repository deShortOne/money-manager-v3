
using MoneyTracker.Calculation.Bill;
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Bill.Tests.Service;
public sealed class SkipOccurenceTest
{
    [Fact]
    public async void SuccessfullySkipOccurenceInBill()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var dateToEvaluate = new DateOnly(2024, 10, 8);
        var dateToBecome = new DateOnly(2024, 11, 8);
        var skipBillOccurence = new SkipBillOccurrenceRequestDTO(billId, dateToEvaluate);
        var frequencyToCheck = "POLEMNB";
        var monthDay = -2;
        var editBillEntity = new EditBillEntity(billId, nextDueDate: dateToBecome);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(true));
        mockBillDatabase.Setup(x => x.GetBillById(authedUser, billId))
            .Returns(Task.FromResult(new BillEntityDTO(userId, "", 0, new DateOnly(), frequencyToCheck, "", monthDay, "")));
        mockBillDatabase.Setup(x => x.EditBill(editBillEntity));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var mockCategoryDatabase = new Mock<ICategoryDatabase>();

        var mockFrequencyCalculation = new Mock<IFrequencyCalculation>();
        mockFrequencyCalculation.Setup(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate))
            .Returns(dateToBecome);

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object,
            mockIdGenerator.Object,
            mockFrequencyCalculation.Object,
            new MonthDayCalculator(),
            mockCategoryDatabase.Object);

        await billService.SkipOccurence(tokenToDecode, skipBillOccurence);
        Assert.Multiple(() =>
        {
            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
            mockBillDatabase.Verify(x => x.GetBillById(authedUser, billId), Times.Once);
            mockBillDatabase.Verify(x => x.EditBill(editBillEntity), Times.Once);
        });
    }

    [Fact]
    public void BillDoesntBelongToUser_Fails()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var billId = 1;
        var dateToEvaluate = new DateOnly(2024, 10, 8);
        var dateToBecome = new DateOnly(2024, 11, 8);
        var skipBillOccurence = new SkipBillOccurrenceRequestDTO(billId, dateToEvaluate);
        var frequencyToCheck = "POLEMNB";
        var monthDay = -2;
        var editBillEntity = new EditBillEntity(billId, nextDueDate: dateToBecome);

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.IsBillAssociatedWithUser(authedUser, billId)).Returns(Task.FromResult(false));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var mockCategoryDatabase = new Mock<ICategoryDatabase>();

        var mockFrequencyCalculation = new Mock<IFrequencyCalculation>();
        mockFrequencyCalculation.Setup(x => x.CalculateNextDueDate(frequencyToCheck, monthDay, dateToEvaluate))
            .Returns(dateToBecome);

        var billService = new BillService(mockBillDatabase.Object,
            mockDateProvider.Object,
            mockUserAuthService.Object,
            mockAccountDatabase.Object,
            mockIdGenerator.Object,
            mockFrequencyCalculation.Object,
            new MonthDayCalculator(),
            mockCategoryDatabase.Object);

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await billService.SkipOccurence(tokenToDecode, skipBillOccurence);
            });
            Assert.Equal("Bill not found", error.Message);

            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            mockBillDatabase.Verify(x => x.IsBillAssociatedWithUser(authedUser, billId), Times.Once);
        });
    }
}
