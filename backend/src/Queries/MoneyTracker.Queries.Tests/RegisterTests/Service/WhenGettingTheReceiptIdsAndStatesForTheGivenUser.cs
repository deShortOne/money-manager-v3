
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Contracts.Responses.Receipt;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service;
public class WhenGettingTheReceiptIdsAndStatesForTheGivenUser : RegisterTestHelper, IAsyncLifetime
{
    private const string _token = "da tokenator";
    private const int _userId = 245;
    private readonly List<ReceiptIdAndStateEntity> _receiptEntities = [
        new ReceiptIdAndStateEntity("pafew;uionjhs", ReceiptState.Finished),
        new ReceiptIdAndStateEntity("auveirn;hplj", ReceiptState.Unknown)
    ];

    private AuthenticatedUser _resultAuthenticatedUser;
    private List<ReceiptState> _resultListReceiptState;
    private ResultT<List<ReceiptIdAndStateResponse>> _result;

    public async Task InitializeAsync()
    {
        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.User)
            .Returns(new UserEntity(_userId, "", ""));
        _mockUserRepository
            .Setup(x => x.GetUserAuthFromToken(_token, CancellationToken.None))
            .ReturnsAsync(mockUserAuth.Object);

        _mockRegisterDatabase
            .Setup(x => x.GetReceiptStatesForUser(It.IsAny<AuthenticatedUser>(), It.IsAny<List<ReceiptState>>(), CancellationToken.None))
            .Callback((AuthenticatedUser authedUser, List<ReceiptState> receiptStates, CancellationToken _) =>
            {
                _resultAuthenticatedUser = authedUser;
                _resultListReceiptState = receiptStates;
            })
            .ReturnsAsync(_receiptEntities);

        _result = await _registerService.GetReceiptsAndStatesForGivenUser(_token, CancellationToken.None);

    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheCorrectUserIsPassedIntoTheRepository()
    {
        Assert.Equal(_userId, _resultAuthenticatedUser.Id);
    }

    [Fact]
    public void ThenTheCorrectReceiptStatesIsPassedIntoTheRepository()
    {
        Assert.Equal([ReceiptState.Processing, ReceiptState.Pending], _resultListReceiptState);
    }

    [Fact]
    public void ThenTheEntitiesAreMappedCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(_receiptEntities[0].Id, _result.Value[0].Id);
            Assert.Equal(_receiptEntities[0].State, _result.Value[0].State);

            Assert.Equal(_receiptEntities[1].Id, _result.Value[1].Id);
            Assert.Equal(_receiptEntities[1].State, _result.Value[1].State);
        });
    }
}
