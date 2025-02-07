using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.DatabaseOnlyRepositoryService;
public class DatabaseOnlyTestHelper
{
    protected Mock<IAccountDatabase> _mockAccountDatabase;

    protected AccountRepository _accountRepositoryService;

    protected DatabaseOnlyTestHelper()
    {
        _mockAccountDatabase = new Mock<IAccountDatabase>();

        _accountRepositoryService = new AccountRepository(
            _mockAccountDatabase.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockAccountDatabase.VerifyNoOtherCalls();
    }
}
