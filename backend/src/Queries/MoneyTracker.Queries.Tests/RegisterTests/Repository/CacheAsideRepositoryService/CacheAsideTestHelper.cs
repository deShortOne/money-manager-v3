using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.CacheAsideRepositoryService;
public abstract class CacheAsideTestHelper : IAsyncLifetime
{
    protected Mock<IRegisterDatabase> _mockRegisterDatabase;
    protected Mock<IRegisterCache> _mockRegisterCache;

    protected RegisterRepository _registerRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockRegisterDatabase = new Mock<IRegisterDatabase>();
        _mockRegisterCache = new Mock<IRegisterCache>();

        _registerRepositoryService = new RegisterRepository(
            _mockRegisterDatabase.Object,
            _mockRegisterCache.Object
        );
    }

    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();

    protected void VerifyNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
        _mockRegisterCache.VerifyNoOtherCalls();
    }
}
