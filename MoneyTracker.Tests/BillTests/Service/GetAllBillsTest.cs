using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
using Moq;

namespace MoneyTracker.Tests.BillTests.Service;
public sealed class GetAllBillsTest : BillTestHelper
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

        _mockUserAuthService.Setup(x => x.DecodeToken(tokenToDecode)).Returns(Task.FromResult(authedUser));

        _mockBillDatabase.Setup(x => x.GetAllBills(authedUser)).Returns(Task.FromResult(billDatabaseReturn));

        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8), _mockDateProvider.Object)).Returns((OverDueBillInfo?)null);
        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23), _mockDateProvider.Object)).Returns(secondResponseOverdueBillInfo);

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _billService.GetAllBills(tokenToDecode));

            _mockUserAuthService.Verify(x => x.DecodeToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.GetAllBills(authedUser), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8), _mockDateProvider.Object), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23), _mockDateProvider.Object), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
