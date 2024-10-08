
using MoneyTracker.Calculation.Bill;
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Bill.Tests.Service;
public sealed class GetAllBillsTest
{
    [Fact]
    public void SuccessfullyGetBills()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var secondResponseOverdueBillInfo = new OverDueBillInfo(5, []);
        List<BillEntityDTO> billDatabaseReturn = [
            new(1, "fds", 16, new DateOnly(2024, 10, 8), "Daily", "Category", 8, "account"),
            new(2, "jgf", 999, new DateOnly(2023, 4, 23), "Weekly", "Hobby", 23, "account"),
        ];
        List<BillResponseDTO> expected = [
            new(1, "fds", 16, new DateOnly(2024, 10, 8), "Daily", "Category", null, "account"),
            new(2, "jgf", 999, new DateOnly(2023, 4, 23), "Weekly", "Hobby", secondResponseOverdueBillInfo, "account"),
        ];

        var mockDateProvider = new Mock<IDateProvider>();

        var mockUserAuthService = new Mock<IUserAuthenticationService>();
        mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        var mockAccountDatabase = new Mock<IAccountDatabase>();

        var mockBillDatabase = new Mock<IBillDatabase>();
        mockBillDatabase.Setup(x => x.GetAllBills(authedUser)).Returns(Task.FromResult(billDatabaseReturn));

        var mockIdGenerator = new Mock<IIdGenerator>();

        var mockCategoryDatabase = new Mock<ICategoryDatabase>();

        var mockFrequencyCalculation = new Mock<IFrequencyCalculation>();
        mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8), mockDateProvider.Object)).Returns<OverDueBillInfo?>(null);
        mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23), mockDateProvider.Object)).Returns(secondResponseOverdueBillInfo);

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
            Assert.Equal(expected, await billService.GetAllBills(tokenToDecode));

            mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            mockBillDatabase.Verify(x => x.GetAllBills(authedUser), Times.Once);
        });
    }
}
