using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public abstract class DatabaseOnlyTestHelper : IAsyncLifetime
{
    protected Mock<IRegisterDatabase> _mockRegisterDatabase;

    protected RegisterRepository _registerRepositoryService;

    protected DatabaseOnlyTestHelper()
    {
        _mockRegisterDatabase = new Mock<IRegisterDatabase>();

        _registerRepositoryService = new RegisterRepository(
            _mockRegisterDatabase.Object
        );
    }

    public abstract Task InitializeAsync();
    public abstract Task DisposeAsync();

    protected void VerifyNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
    }
}
