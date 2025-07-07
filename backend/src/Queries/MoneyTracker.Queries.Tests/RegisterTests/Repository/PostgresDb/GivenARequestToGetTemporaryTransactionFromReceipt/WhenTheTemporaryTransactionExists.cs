
using System.Data.Common;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Npgsql;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetTemporaryTransactionFromReceipt;
public class WhenTheTemporaryTransactionExists : TemporaryTransactionHelper
{
    private readonly int _userId = 1;
    private readonly string _filename = "file name";
    private readonly AccountResponse? _payee = null;
    private readonly decimal? _amount = 43m;
    private readonly DateOnly? _datePaid = null;
    private readonly CategoryResponse? _category = null;
    private readonly AccountResponse? _payer = null;

    private ResultT<TemporaryTransaction> _result;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await SetupDatabase();

        _result = await _registerDatabase.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None);
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
            Assert.Equal(_userId, entity.UserId);
            Assert.Equal(_filename, entity.Filename);
            Assert.Equal(_payee, entity.Payee);
            Assert.Equal(_amount, entity.Amount);
            Assert.Equal(_datePaid, entity.DatePaid);
            Assert.Equal(_category, entity.Category);
            Assert.Equal(_payer, entity.Payer);
        });
    }

    private async Task SetupDatabase()
    {
        var query = """
            INSERT INTO receipt_to_register (users_id, filename, payee, amount, datepaid, category_id, account_id) VALUES
                (@users_id, @filename, @payee, @amount, @datepaid, @category_id, @account_id);
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("users_id", _userId),
            new NpgsqlParameter("filename", _filename),
            new NpgsqlParameter<int?>("payee", _payee?.Id),
            new NpgsqlParameter<decimal?>("amount", _amount),
            new NpgsqlParameter<DateOnly?>("datepaid", _datePaid),
            new NpgsqlParameter<int?>("category_id", _category?.Id),
            new NpgsqlParameter<int?>("account_id", _payer?.Id),
        };

        await _database.UpdateTable(query, CancellationToken.None, queryParams);
    }
}
