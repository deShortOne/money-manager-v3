using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Service;
public class BillTestHelper
{
    public readonly Mock<IBillRepository> _mockBillDatabase = new();
    public readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();
    public readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();

    public readonly BillService _billService;

    public BillTestHelper()
    {
        _billService = new BillService(_mockBillDatabase.Object,
            _mockDateTimeProvider.Object,
            _mockUserAuthService.Object,
            _mockFrequencyCalculation.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockDateTimeProvider.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
    }
}
