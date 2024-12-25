using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public class RegisterTestHelper
{
    public readonly Mock<IRegisterCommandRepository> _mockRegisterDatabase = new();
    public readonly Mock<IAccountCommandRepository> _mockAccountDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IUserCommandRepository> _mockUserRepository = new();

    public readonly RegisterService _registerService;

    public RegisterTestHelper()
    {
        _registerService = new RegisterService(
            _mockRegisterDatabase.Object,
            _mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockUserRepository.Object
        );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockUserRepository.VerifyNoOtherCalls();
    }
}
