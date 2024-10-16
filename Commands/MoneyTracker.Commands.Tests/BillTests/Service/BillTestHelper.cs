using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public class BillTestHelper
{
    public readonly Mock<IBillCommandRepository> _mockBillDatabase = new();
    public readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();
    public readonly Mock<IAccountCommandRepository> _mockAccountDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();
    public readonly Mock<IMonthDayCalculator> _mockMonthDayCalculator = new();
    public readonly Mock<ICategoryCommandRepository> _mockCategoryDatabase = new();

    public readonly BillService _billService;

    public BillTestHelper()
    {
        _billService = new BillService(_mockBillDatabase.Object,
            _mockDateTimeProvider.Object,
            _mockUserAuthService.Object,
            _mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockFrequencyCalculation.Object,
            _mockMonthDayCalculator.Object,
            _mockCategoryDatabase.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockDateTimeProvider.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
        _mockMonthDayCalculator.VerifyNoOtherCalls();
        _mockCategoryDatabase.VerifyNoOtherCalls();
    }
}
