
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetReceiptProcessingInfo;
public class WhenTheReceiptDoesntExist : ReceiptProcessingInfoHelper
{
    private readonly string _filename = "bad file name";
    private readonly string _errorMessage;

    private ResultT<ReceiptEntity> _result;

    public WhenTheReceiptDoesntExist()
    {
        _errorMessage = $"Could not find receipt procesing information for given id: {_filename}";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _result = await _registerDatabase.GetReceiptProcessingInfo(_filename, CancellationToken.None);
    }

    [Fact]
    public void ThenErrorsAreRaised()
    {
        Assert.True(_result.HasError);
        Assert.Equal(_errorMessage, _result.Error!.Description);
    }
}
