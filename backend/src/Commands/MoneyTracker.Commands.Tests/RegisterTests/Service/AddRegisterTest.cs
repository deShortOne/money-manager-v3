using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public sealed class AddRegisterTest : RegisterTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _lastTransactionId = 2;
    private readonly int _newTransactionId = 1;
    private readonly int _payeeId = 87;
    private readonly decimal _amount = 25;
    private readonly DateOnly _datePaid = new DateOnly(2024, 12, 8);
    private readonly int _categoryId = 2;
    private readonly int _payerId = 15;

    public AddRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    [Fact]
    public async Task SuccessfullyAddNewTransaction()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 3, _userId, false));

        _mockCategoryService
            .Setup(x => x.DoesCategoryExist(_categoryId))
            .ReturnsAsync(true);

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_categoryId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetLastTransactionId(), Times.Once);
            _mockRegisterDatabase.Verify(x => x.AddTransaction(newTransaction), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastTransactionId), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                new EventUpdate(_authedUser, DataTypes.Register), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDueToPayerAccountNotExisting()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync((AccountUserEntity)null);

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDueToUserNotOwningPayerAccount()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId, false));

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDuePayerToAccountNotBelongingToUser()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId + 1, true));

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payer account not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDueToPayeeAccountNotExisting()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));

        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);

        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync((AccountUserEntity)null);

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDueToPayeeAccountNotBelongingToUser()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));

        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);

        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 1, _userId + 1, true));

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Payee account not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailsDueToCategoryNotExisting()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payerId))
            .ReturnsAsync(new AccountUserEntity(_payerId, 1, _userId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(_payeeId))
            .ReturnsAsync(new AccountUserEntity(_payeeId, 3, _userId, false));

        _mockCategoryService
            .Setup(x => x.DoesCategoryExist(_categoryId))
            .ReturnsAsync(false);

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        var error = await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            Assert.Equal("Category not found", error.Error.Description);

            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payerId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountUserEntity(_payeeId), Times.Once);
            _mockCategoryService.Verify(x => x.DoesCategoryExist(_categoryId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
