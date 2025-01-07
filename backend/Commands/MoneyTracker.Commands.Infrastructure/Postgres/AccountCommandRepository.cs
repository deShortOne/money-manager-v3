using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
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

    public async Task<bool> IsAccountOwnedByUser(AuthenticatedUser user, int accountId)
    {
        var query = """
            SELECT 1
            FROM account
            WHERE users_id = @userid
            AND id = @account_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userid", user.Id),
            new NpgsqlParameter("account_id", accountId),
        };

        var reader = await _database.GetTable(query, queryParams);

        return reader.Rows.Count != 0 && reader.Rows[0].Field<int>(0) == 1;
    }

    public async Task<bool> IsValidAccount(int accountId)
    {
        var query = """
            SELECT 1
            FROM account
            WHERE id = @account_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("account_id", accountId),
        };

        var reader = await _database.GetTable(query, queryParams);

        return reader.Rows.Count != 0 && reader.Rows[0].Field<int>(0) == 1;
    }
}
