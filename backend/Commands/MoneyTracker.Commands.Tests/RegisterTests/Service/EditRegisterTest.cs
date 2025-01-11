
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Utilities.DateTimeUtil;
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

    public static TheoryData<int?, int?, DateOnly?, int?, int?> OnlyOneItemNotNull = new() {
        { 745, null, null, null, null },
        { null, 500, null, null, null },
        { null, null, new DateOnly(2023, 2, 1), null, null },
        { null, null, null, 3, null },
        { null, null, null, null, 1 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditTransactionOneItemOnly(int? payee, int? amount, DateOnly? datePaid, int? categoryId, int? payerId)
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId)).Returns(Task.FromResult(true));
        if (payerId != null)
            _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, (int)payerId)).Returns(Task.FromResult(true));

        var editTransactionRequest = new EditTransactionRequest(_transactionId, payee, amount, datePaid, categoryId, payerId);
        var editTransaction = new EditTransactionEntity(_transactionId, payee, amount, datePaid, categoryId, payerId);

        await _registerService.EditTransaction(_tokenToDecode, editTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.EditTransaction(editTransaction), Times.Once);

            if (payerId != null)
                _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, (int)payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
