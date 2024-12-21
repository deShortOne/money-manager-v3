using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public class UserTestHelper
{
    public readonly Mock<IUserCommandRepository> _mockUserDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();

    public readonly UserService _userService;

    public UserTestHelper()
    {
        _userService = new UserService(
            _mockUserDatabase.Object, 
            _mockIdGenerator.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockUserDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
    }
}
