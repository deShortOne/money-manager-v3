using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.PlatformService.Domain;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public class RegisterTestHelper
{
    public readonly Mock<IRegisterCommandRepository> _mockRegisterDatabase = new();
    public readonly Mock<IAccountCommandRepository> _mockAccountDatabase = new();
    public readonly Mock<IIdGenerator> _mockIdGenerator = new();
    public readonly Mock<IUserService> _mockUserService = new();
    public readonly Mock<IAccountService> _accountService = new();
    public readonly Mock<ICategoryService> _mockCategoryService = new();
    public readonly Mock<IMessageBusClient> _mockMessageBusClient = new();
    public readonly Mock<IFileUploadRepository> _mockFileUploadRepository = new();
    public readonly Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    public readonly Mock<IReceiptCommandRepository> _mockReceiptCommandRepository = new();


    public readonly RegisterService _registerService;

    public RegisterTestHelper()
    {
        _registerService = new RegisterService(
            _mockRegisterDatabase.Object,
            _mockAccountDatabase.Object,
            _mockIdGenerator.Object,
            _mockUserService.Object,
            _accountService.Object,
            _mockCategoryService.Object,
            _mockMessageBusClient.Object,
            _mockFileUploadRepository.Object,
            _mockDateTimeProvider.Object,
            _mockReceiptCommandRepository.Object
            );
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockRegisterDatabase.VerifyNoOtherCalls();
        _mockAccountDatabase.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockUserService.VerifyNoOtherCalls();
        _accountService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyNoOtherCalls();
        _mockMessageBusClient.VerifyNoOtherCalls();
        _mockFileUploadRepository.VerifyNoOtherCalls();
        _mockDateTimeProvider.VerifyNoOtherCalls();
        _mockReceiptCommandRepository.VerifyNoOtherCalls();
    }
}
