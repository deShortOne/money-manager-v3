using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public class BudgetTestHelper
{
    public readonly Mock<IUserAuthenticationService> _mockUserAuthService = new();
    public readonly Mock<IBudgetCommandRepository> _mockBudgetCategoryDatabase = new();

    public readonly BudgetService _budgetService;

    public BudgetTestHelper()
    {
        _budgetService = new BudgetService(
            _mockUserAuthService.Object,
            _mockBudgetCategoryDatabase.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBudgetCategoryDatabase.VerifyNoOtherCalls();
        _mockUserAuthService.VerifyNoOtherCalls();
    }
}
