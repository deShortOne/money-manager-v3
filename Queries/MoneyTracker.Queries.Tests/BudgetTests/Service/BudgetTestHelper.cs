using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Service;
public class BudgetTestHelper
{
    public readonly Mock<IBudgetRepository> _mockBudgetDatabase = new();
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();

    public readonly BudgetService _budgetService;

    public BudgetTestHelper()
    {
        _budgetService = new BudgetService(_mockUserAuthService.Object,
            _mockBudgetDatabase.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBudgetDatabase.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
    }
}
