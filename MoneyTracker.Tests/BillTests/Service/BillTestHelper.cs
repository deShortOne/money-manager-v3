using MoneyTracker.Calculation.Bill;
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Bill.Tests.Service;
public class BillTestHelper
{
    public readonly Mock<IDateProvider> _mockDateProvider = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();
    public readonly Mock<IAccountDatabase> _mockAccountDatabase = new();
    public readonly Mock<IBillDatabase> _mockBillDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();
    public readonly Mock<IMonthDayCalculator> _mockMonthDayCalculator = new();
    public readonly Mock<ICategoryDatabase> _mockCategoryDatabase = new();

    public readonly BillService _billService;

    public BillTestHelper()
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

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockDateProvider.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
        _mockMonthDayCalculator.VerifyNoOtherCalls();
        _mockCategoryDatabase.VerifyNoOtherCalls();
    }
}
