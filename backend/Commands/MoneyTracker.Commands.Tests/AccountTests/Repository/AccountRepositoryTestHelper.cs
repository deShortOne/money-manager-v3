using System.Data;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class AccountRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IAccountCommandRepository _accountRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _accountRepo = new AccountCommandRepository(_database);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    protected async Task<List<AccountEntity>> GetAllAccountEntity()
    {
        var getBillQuery = @"
                            SELECT id, name
                            FROM account;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBillInfo = new NpgsqlCommand(getBillQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBillInfo.ExecuteReader();
        List<AccountEntity> results = [];
        while (reader.Read())
        {
            results.Add(new AccountEntity(
                id: reader.GetInt32("id"),
                name: reader.GetString("name")
            ));
        }
        return results;
    }

    protected async Task<List<AccountUserEntity>> GetAllAccountUserEntity()
    {
        var getBillQuery = @"
                            SELECT id,
                                users_id,
                                account_id,
                                user_owns_account
                            FROM account_user;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBillInfo = new NpgsqlCommand(getBillQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBillInfo.ExecuteReader();
        List<AccountUserEntity> results = [];
        while (reader.Read())
        {
            results.Add(new AccountUserEntity(
                id: reader.GetInt32("id"),
                accountId: reader.GetInt32("account_id"),
                userId: reader.GetInt32("users_id"),
                userOwnsAccount: reader.GetBoolean("user_owns_account")
            ));
        }
        return results;
    }
}
