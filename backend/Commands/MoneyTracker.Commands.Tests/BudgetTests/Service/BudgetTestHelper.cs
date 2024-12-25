using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using Moq;

namespace MoneyTracker.Commands.Tests.BudgetTests.Service;
public class BudgetTestHelper
{
    public readonly Mock<IBudgetCommandRepository> _mockBudgetCategoryDatabase = new();
    public readonly Mock<IUserCommandRepository> _mockUserRepository = new();

    public readonly BudgetService _budgetService;

    public BudgetTestHelper()
    {
        _budgetService = new BudgetService(
            _mockBudgetCategoryDatabase.Object,
            _mockUserRepository.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBudgetCategoryDatabase.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
