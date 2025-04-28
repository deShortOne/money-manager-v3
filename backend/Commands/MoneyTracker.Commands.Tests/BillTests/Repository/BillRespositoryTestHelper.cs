using System.Data;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public class BillRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IBillCommandRepository _billRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _billRepo = new BillCommandRepository(_database);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    protected async Task<List<BillEntity>> GetAllBillEntity()
    {
        var getBillQuery = @"
                            SELECT id, payee_user_id, amount, nextduedate, frequency, category_id, monthday, payer_user_id
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
                payeeId: reader.GetInt32("payee_user_id"),
                amount: reader.GetDecimal("amount"),
                nextDueDate: DateOnly.FromDateTime(reader.GetDateTime("nextduedate")),
                monthDay: reader.GetInt32("monthday"),
                frequency: reader.GetString("frequency"),
                categoryId: reader.GetInt32("category_id"),
                payerId: reader.GetInt32("payer_user_id"))
            );
        }
        return results;
    }
}
