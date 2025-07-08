
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.DatabaseOnlyRepositoryService;
public class WhenRequestingReceiptsOfStateForAUser : DatabaseOnlyTestHelper
{
    private AuthenticatedUser _user = new AuthenticatedUser(1);
    private List<int> _designatedStates = [1, 3, 4];
    private List<ReceiptIdAndStateEntity> _receiptIdAndStateEntities = new List<ReceiptIdAndStateEntity>();

    private List<ReceiptIdAndStateEntity> _result;

    public override async Task InitializeAsync()
    {
        _mockRegisterDatabase
            .Setup(x => x.GetReceiptStatesForUser(_user, _designatedStates, CancellationToken.None))
            .ReturnsAsync(_receiptIdAndStateEntities);

        _result = await _registerRepositoryService.GetReceiptStatesForUser(_user, _designatedStates, CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenDatabaseIsCalledOnce()
    {
        _mockRegisterDatabase.Verify(x => x.GetReceiptStatesForUser(_user, _designatedStates, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheDataIsMappedCorrectly()
    {
        Assert.Equal(_receiptIdAndStateEntities, _result);
    }
}
