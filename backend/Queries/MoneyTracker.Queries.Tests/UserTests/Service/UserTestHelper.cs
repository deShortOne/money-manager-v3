using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public class UserTestHelper
{
    public readonly Mock<IUserDatabase> _mockUserDatabase = new();
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
