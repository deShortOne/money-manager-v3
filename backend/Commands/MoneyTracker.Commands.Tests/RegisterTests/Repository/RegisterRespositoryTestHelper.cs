using System.Data;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public class RegisterRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IRegisterCommandRepository _registerRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _registerRepo = new RegisterCommandRepository(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    protected async Task<List<TransactionEntity>> GetAllTransactionEntities()
    {
        var getBudgetQuery = @"
                            SELECT id, payee, amount, datepaid, category_id, account_id
                            FROM register
                            ORDER BY id desc;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBudgetInfo = new NpgsqlCommand(getBudgetQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBudgetInfo.ExecuteReader();
        List<TransactionEntity> results = [];
        while (reader.Read())
        {
            results.Add(new TransactionEntity(id: reader.GetInt32("id"),
                payee: reader.GetInt32("payee"),
                amount: reader.GetDecimal("amount"),
                datePaid: DateOnly.FromDateTime(reader.GetDateTime("datepaid")),
                categoryId: reader.GetInt32("category_id"),
                accountId: reader.GetInt32("account_id")
            ));
        }
        return results;
    }
}
