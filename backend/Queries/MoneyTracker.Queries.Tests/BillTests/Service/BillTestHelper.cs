using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Service;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Service;
public class BillTestHelper
{
    public readonly Mock<IBillRepositoryService> _mockBillDatabase = new();
    public readonly Mock<IFrequencyCalculation> _mockFrequencyCalculation = new();
    public readonly Mock<IUserRepositoryService> _mockUserRepository = new();

    public readonly BillService _billService;

    public BillTestHelper()
    {
        _billService = new BillService(_mockBillDatabase.Object,
            _mockFrequencyCalculation.Object,
            _mockUserRepository.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockFrequencyCalculation.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
