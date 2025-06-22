using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.CacheAsideRepositoryService;
public class CacheAsideTestHelper
{
    protected Mock<IAccountDatabase> _mockAccountDatabase;
    protected Mock<IAccountCache> _mockAccountCache;

    protected AccountRepository _accountRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockAccountDatabase = new Mock<IAccountDatabase>();
        _mockAccountCache = new Mock<IAccountCache>();

        _accountRepositoryService = new AccountRepository(
            _mockAccountDatabase.Object,
            _mockAccountCache.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockAccountCache.VerifyNoOtherCalls();
    }
}
