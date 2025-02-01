using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Service;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Service;
public class BudgetTestHelper
{
    public readonly Mock<IBudgetRepositoryService> _mockBudgetDatabase = new();
    public readonly Mock<IUserRepositoryService> _mockUserRepository = new();

    public readonly BudgetService _budgetService;

    public BudgetTestHelper()
    {
        _budgetService = new BudgetService(
            _mockBudgetDatabase.Object,
            _mockUserRepository.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockBudgetDatabase.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
