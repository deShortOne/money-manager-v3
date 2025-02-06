using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.CacheAsideRepositoryService;
public class CacheAsideTestHelper
{
    protected Mock<IUserDatabase> _mockUserDatabase;

    protected UserRepository _userRepositoryService;

    protected CacheAsideTestHelper()
    {
        _mockUserDatabase = new Mock<IUserDatabase>();

        _userRepositoryService = new UserRepository(
            _mockUserDatabase.Object
        );
    }

    protected void VerifyNoOtherCalls()
    {
        _mockUserDatabase.VerifyNoOtherCalls();
    }
}
