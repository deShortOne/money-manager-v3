
using System.Data.Common;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Npgsql;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetReceiptProcessingInfo;
public class WhenAReceiptExists : ReceiptProcessingInfoHelper
{
    private readonly string _id = "8A26CD22-5518-4D35-A20D-8C059A129299";
    private readonly int _userId = 1;
    private readonly string _filename = "file name";
    private readonly string _url = "da url";
    private readonly ReceiptState _state = ReceiptState.Pending;


    private ResultT<ReceiptEntity> _result;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await SetupDatabase();

        _result = await _registerDatabase.GetReceiptProcessingInfo(_id, CancellationToken.None);
    }

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheReceiptEntityIsMappedCorrectly()
    {
        var entity = _result.Value;
        Assert.Multiple(() =>
        {
            Assert.Equal(_id, entity.Id);
            Assert.Equal(_userId, entity.UserId);
            Assert.Equal(_filename, entity.Name);
            Assert.Equal(_url, entity.Url);
            Assert.Equal(_state, entity.State);
        });
    }

    private async Task SetupDatabase()
    {
        var insertQuery = """
            INSERT INTO receipt_analysis_state (id, users_id, filename, url, state) VALUES
            (@id, @users_id, @filename, @url, @state);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", _id),
            new NpgsqlParameter("users_id", _userId),
            new NpgsqlParameter("filename", _filename),
            new NpgsqlParameter("url", _url),
            new NpgsqlParameter("state", (int)_state),
        };

        await _database.UpdateTable(insertQuery, CancellationToken.None, queryParams);
    }
}
