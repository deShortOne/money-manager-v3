using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.CacheAsideRepositoryService;
public class CacheAsideTestHelper
{
    protected Mock<IBillDatabase> _mockBillDatabase;
    protected Mock<IBillCache> _mockBillCache;

    protected BillRepository _billRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockBillDatabase = new Mock<IBillDatabase>();
        _mockBillCache = new Mock<IBillCache>();

        _billRepositoryService = new BillRepository(
            _mockBillDatabase.Object,
            _mockBillCache.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockBillDatabase.VerifyNoOtherCalls();
        _mockBillCache.VerifyNoOtherCalls();
    }
}
