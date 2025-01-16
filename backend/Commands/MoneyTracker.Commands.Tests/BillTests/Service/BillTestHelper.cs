using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.BillTests.Service;
public class BillTestHelper
{
    public readonly Mock<IBillCommandRepository> _mockBillDatabase = new();
    public readonly Mock<IAccountCommandRepository> _mockAccountDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();
    public readonly Mock<IMonthDayCalculator> _mockMonthDayCalculator = new();
    public readonly Mock<ICategoryService> _mockCategoryService = new();
    public readonly Mock<IUserService> _mockUserService = new();
    public readonly Mock<IAccountService> _mockAccountService = new();

    public readonly BillService _billService;

    public BillTestHelper()
    {
        _billService = new BillService(_mockBillDatabase.Object,
            _mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockFrequencyCalculation.Object,
            _mockMonthDayCalculator.Object,
            _mockCategoryService.Object,
            _mockUserService.Object,
            _mockAccountService.Object
            );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
        _mockMonthDayCalculator.VerifyNoOtherCalls();
        _mockCategoryService.VerifyNoOtherCalls();
        _mockUserService.VerifyNoOtherCalls();
        _mockAccountService.VerifyNoOtherCalls();
    }
}
