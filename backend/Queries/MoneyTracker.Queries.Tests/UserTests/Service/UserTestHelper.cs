using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Domain.Repositories.Service;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public class UserTestHelper
{
    public readonly Mock<IUserRepositoryService> _mockUserDatabase = new();
    public readonly Mock<IPasswordHasher> _mockPasswordHasher = new();

    public readonly UserService _userService;

    public UserTestHelper()
    {
        _userService = new UserService(_mockUserDatabase.Object, _mockPasswordHasher.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockUserDatabase.VerifyNoOtherCalls();
        _mockPasswordHasher.VerifyNoOtherCalls();
    }
}
