
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public sealed class EditRegisterTest : RegisterTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";
    
    private readonly int _transactionId = 2;

    public EditRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    public static TheoryData<string, int?, DateOnly?, int?, int?> OnlyOneItemNotNull = new() {
        { "payee", null, null, null, null },
        { null, 500, null, null, null },
        { null, null, new DateOnly(2023, 2, 1), null, null },
        { null, null, null, 3, null },
        { null, null, null, null, 1 },
    };

    [Theory,  MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditTransactionOneItemOnly(string payee, int? amount, DateOnly? datePaid, int? categoryId, int? accountId)
    {
        _mockUserAuthService.Setup(x => x.DecodeToken(_tokenToDecode)).Returns(Task.FromResult(_authedUser));
        _mockRegisterDatabase.Setup(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId)).Returns(Task.FromResult(true));
        if (accountId != null)
            _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, (int)accountId)).Returns(Task.FromResult(true));

        var editTransactionRequest = new EditTransactionRequest(_transactionId, payee, amount, datePaid, categoryId, accountId);
        var editTransaction = new EditTransactionEntity(_transactionId, payee, amount, datePaid, categoryId, accountId);

        await _registerService.EditTransaction(_tokenToDecode, editTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.EditTransaction(editTransaction), Times.Once);
            
            if (accountId != null)
                _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, (int)accountId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
