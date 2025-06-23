using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Bill;
using MoneyTracker.Contracts.Responses.Category;
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
            new(1, 43, "fds", 16, new DateOnly(2024, 10, 8), 8, "Daily", 53, "Category", 28, "account"),
            new(2, 43, "jgf", 999, new DateOnly(2023, 4, 23), 23, "Weekly", 52, "Hobby", 38, "account"),
        ];
        List<BillResponse> expected = [
            new(1, new AccountResponse(43, "fds"), 16, new DateOnly(2024, 10, 8), "Daily", new CategoryResponse(53, "Category"), null, new AccountResponse(28, "account")),
            new(2, new AccountResponse(43, "jgf"), 999, new DateOnly(2023, 4, 23), "Weekly", new CategoryResponse(52, "Hobby"), secondResponseOverdueBillInfo, new AccountResponse(38, "account")),
        ];

        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.CheckValidation()).Returns(Result.Success());
        mockUserAuth.Setup(x => x.User).Returns(new UserEntity(userId, "", ""));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(tokenToDecode))
            .ReturnsAsync(mockUserAuth.Object);

        _mockBillDatabase.Setup(x => x.GetAllBills(authedUser)).ReturnsAsync(billDatabaseReturn);

        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8)))
            .Returns((OverDueBillInfo?)null);
        _mockFrequencyCalculation.Setup(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23)))
            .Returns(secondResponseOverdueBillInfo);

        Assert.Multiple(async () =>
        {
            var a = await _billService.GetAllBills(tokenToDecode);
            Assert.Equal(expected, a);

            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(tokenToDecode), Times.Once);
            _mockBillDatabase.Verify(x => x.GetAllBills(authedUser), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(8, "Daily", new DateOnly(2024, 10, 8)), Times.Once);
            _mockFrequencyCalculation.Verify(x => x.CalculateOverDueBillInfo(23, "Weekly", new DateOnly(2023, 4, 23)), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
