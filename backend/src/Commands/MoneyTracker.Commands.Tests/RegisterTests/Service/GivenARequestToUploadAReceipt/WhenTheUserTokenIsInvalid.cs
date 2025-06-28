
using Microsoft.AspNetCore.Http;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service.GivenARequestToUploadAReceipt;
public class WhenTheUserTokenIsInvalid : RegisterTestHelper, IAsyncLifetime
{
    private string _token = "71AE322B-6A27-46BC-A3FE-577BFE78CB0C";
    private Result _result;

    public async Task InitializeAsync()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_token))
            .ReturnsAsync(Error.AccessUnAuthorised("asdf", "fdsa"));

        _result = await _registerService.CreateTransactionFromReceipt(_token, Mock.Of<IFormFile>());
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public void ThenThereErrorsAreRaised()
    {
        Assert.True(_result.HasError);
        Assert.Equal(Error.AccessUnAuthorised("asdf", "fdsa"), _result.Error);
    }
}
