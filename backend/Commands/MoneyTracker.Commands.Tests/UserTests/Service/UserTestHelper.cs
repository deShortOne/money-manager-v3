using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.PlatformService.Domain;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public class UserTestHelper
{
    public readonly Mock<IUserCommandRepository> _mockUserDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IPasswordHasher> _mockPasswordHasher = new();
    public readonly Mock<IAuthenticationService> _mockAuthService = new();
    public readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    public readonly Mock<IMessageBusClient> _mockMessageBusClient = new();

    public readonly UserService _userService;

    public UserTestHelper()
    {
        _userService = new UserService(
            _mockUserDatabase.Object,
            _mockIdGenerator.Object,
            _mockPasswordHasher.Object,
            _mockAuthService.Object,
            _mockDateTimeProvider.Object,
            _mockMessageBusClient.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockUserDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockPasswordHasher.VerifyNoOtherCalls();
        _mockAuthService.VerifyNoOtherCalls();
        _mockDateTimeProvider.VerifyNoOtherCalls();
        _mockMessageBusClient.VerifyNoOtherCalls();
    }
}
