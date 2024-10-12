using System.Data;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.BillTests.Repository;
public class BillRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IBillDatabase _billRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _billRepo = new BillDatabase(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    protected async Task<List<BillEntity>> GetAllBillEntity()
    {
        var getBillQuery = @"
                            SELECT id, payee, amount, nextduedate, frequency, category_id, monthday, account_id
                            FROM bill;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBillInfo = new NpgsqlCommand(getBillQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBillInfo.ExecuteReader();
        List<BillEntity> results = [];
        while (reader.Read())
        {
            results.Add(new BillEntity(id: reader.GetInt32("id"),
                payee: reader.GetString("payee"),
                amount: reader.GetDecimal("amount"),
                nextDueDate: DateOnly.FromDateTime(reader.GetDateTime("nextduedate")),
                frequency: reader.GetString("frequency"),
                category: reader.GetInt32("category_id"),
                monthDay: reader.GetInt32("monthday"),
                accountId: reader.GetInt32("account_id"))
            );
        }
        return results;
    }
}
