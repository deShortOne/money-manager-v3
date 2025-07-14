
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetTemporaryTransactionFromReceipt;
public class WhenTheTemporaryTransactionDoesntExist : TemporaryTransactionHelper
{
    private readonly string _filename = "file name";
    private readonly string _errorMessage;

    private ResultT<TemporaryTransaction> _result;
    public WhenTheTemporaryTransactionDoesntExist()
    {
        _errorMessage = $"Could not find temporary transaction information for given id: {_filename}";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _result = await _registerDatabase.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None);
    }

    [Fact]
    public void ThenThereAreErrors()
    {
        Assert.True(_result.HasError);
        Assert.Equal(_errorMessage, _result.Error!.Description);
    }
}
