using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Queries.Domain.Entities.Bill;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Service;
public sealed class GetAllBillsTest : BillTestHelper
{
    [Fact]
    public void SuccessfullyGetBills()
    {
        var userId = 52;
        var authedUser = new AuthenticatedUser(userId);
        var tokenToDecode = "tokenToDecode";
        var secondResponseOverdueBillInfo = new OverDueBillInfo(5, []);
        List<BillEntity> billDatabaseReturn = [
            new(1, "fds", 16, new DateOnly(2024, 10, 8), 8, "Daily", "Category", "account"),
            new(2, "jgf", 999, new DateOnly(2023, 4, 23), 23, "Weekly", "Hobby", "account"),
        ];
        List<BillResponse> expected = [
            new(1, "fds", 16, new DateOnly(2024, 10, 8), "Daily", "Category", null, "account"),
            new(2, "jgf", 999, new DateOnly(2023, 4, 23), "Weekly", "Hobby", secondResponseOverdueBillInfo, "account"),
        ];

        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .ReturnsAsync(new UserAuthentication(new UserEntity(userId, "", ""), tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object));

        _mockBillDatabase.Setup(x => x.GetAllBills(authedUser)).Returns(Task.FromResult(billDatabaseReturn));

        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8)))
            .Returns((OverDueBillInfo?)null);
        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23)))
            .Returns(secondResponseOverdueBillInfo);

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await _billService.GetAllBills(tokenToDecode));

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.GetAllBills(authedUser), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8)), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23)), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
