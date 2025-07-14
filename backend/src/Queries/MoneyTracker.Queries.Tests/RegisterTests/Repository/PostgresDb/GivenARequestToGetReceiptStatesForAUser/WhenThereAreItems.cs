
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Values;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetReceiptStatesForAUser;
public class WhenThereAreItems : IAsyncLifetime
{

    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
         .WithDockerEndpoint("tcp://localhost:2375")
#endif
         .WithImage("postgres:16")
         .WithCleanUp(true)
         .Build();

    protected PostgresDatabase _database;
    public RegisterDatabase _registerDatabase;
    private List<ReceiptIdAndStateEntity> _result;

    public virtual async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _registerDatabase = new RegisterDatabase(_database);

        await SetupDatabase();

        _result = await _registerDatabase.GetReceiptStatesForUser(new AuthenticatedUser(UserId), [ReceiptState.Processing, ReceiptState.Pending], CancellationToken.None);
    }

    private const int UserId = 1;

    private const string Id1 = "id 1 receipt";
    private const ReceiptState State1 = ReceiptState.Processing;

    private const string Id2 = "id 24 receipt";
    private const ReceiptState State2 = ReceiptState.Pending;

    private const string Id3 = "id 8945 receipt";
    private const ReceiptState State3 = ReceiptState.Finished;

    private async Task SetupDatabase()
    {
        var query = """
            INSERT INTO receipt_analysis_state (id, users_id, filename, url, state) VALUES
                (@id1, @users_id, 'filename1', 'url1', @state1),
                (@id2, @users_id, 'filename2', 'url2', @state2),
                (@id3, @users_id, 'filename3', 'url3', @state3);
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("users_id", UserId),

            new NpgsqlParameter("id1", Id1),
            new NpgsqlParameter("state1", (int)State1),

            new NpgsqlParameter("id2", Id2),
            new NpgsqlParameter("state2", (int)State2),

            new NpgsqlParameter("id3", Id3),
            new NpgsqlParameter("state3", (int)State3),
        };

        await _database.UpdateTable(query, CancellationToken.None, queryParams);
    }

    public async Task DisposeAsync() => await _postgres.DisposeAsync();


    [Fact]
    public void ThenTheItemsAreReturned()
    {
        Assert.Equal(2, _result.Count);
        Assert.Multiple(() =>
        {
            Assert.Equal(Id1, _result[0].Id);
            Assert.Equal(State1, _result[0].State);

            Assert.Equal(Id2, _result[1].Id);
            Assert.Equal(State2, _result[1].State);
        });
    }

    [Fact]
    public void ThenTheItemWithTheMissingStateIsNotInTheList()
    {
        Assert.DoesNotContain(_result, x => x.Id == Id3);
    }
}
