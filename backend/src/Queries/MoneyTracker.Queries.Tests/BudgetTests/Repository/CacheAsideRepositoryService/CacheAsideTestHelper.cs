using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.CacheAsideRepositoryService;
public class CacheAsideTestHelper
{
    protected Mock<IBudgetDatabase> _mockBudgetDatabase;
    protected Mock<IBudgetCache> _mockBudgetCache;

    protected BudgetRepository _budgetRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockBudgetDatabase = new Mock<IBudgetDatabase>();
        _mockBudgetCache = new Mock<IBudgetCache>();

        _budgetRepositoryService = new BudgetRepository(
            _mockBudgetDatabase.Object,
            _mockBudgetCache.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockBudgetDatabase.VerifyNoOtherCalls();
        _mockBudgetCache.VerifyNoOtherCalls();
    }
}
