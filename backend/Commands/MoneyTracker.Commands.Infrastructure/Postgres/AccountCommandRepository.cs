using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class AccountCommandRepository : IAccountCommandRepository
{
    private readonly IDatabase _database;
    public AccountCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<AccountEntity?> GetAccountById(int accountId)
    {
        var query = """
            SELECT id,
                name,
                users_id
            FROM account
            WHERE id = @account_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("account_id", accountId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new AccountEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<string>("name")!,
                reader.Rows[0].Field<int>("users_id"));
        return null;
    }
}
