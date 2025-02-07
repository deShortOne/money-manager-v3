using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.DatabaseOnlyRepositoryService;
public class DatabaseOnlyTestHelper
{
    protected Mock<IBudgetDatabase> _mockBudgetDatabase;

    protected BudgetRepository _budgetRepositoryService;

    protected DatabaseOnlyTestHelper()
    {
        _mockBudgetDatabase = new Mock<IBudgetDatabase>();

        _budgetRepositoryService = new BudgetRepository(
            _mockBudgetDatabase.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockBudgetDatabase.VerifyNoOtherCalls();
    }
}
