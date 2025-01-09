
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Contracts.Requests.Transaction;
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
    public async void SuccessfullyAddNewTransaction()
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode,
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _payerId)).Returns(Task.FromResult(true));

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetLastTransactionId(), Times.Once);
            _mockRegisterDatabase.Verify(x => x.AddTransaction(newTransaction), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastTransactionId), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
