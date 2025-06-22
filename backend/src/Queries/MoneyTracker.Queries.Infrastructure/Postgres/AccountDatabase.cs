using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class AccountDatabase : IAccountDatabase
{
    private readonly IDatabase _database;
    public AccountDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<ResultT<List<AccountEntity>>> GetAccountsOwnedByUser(AuthenticatedUser user)
    {
        var query = """
            SELECT account_user.id, name
            FROM account
            INNER JOIN account_user
                ON account.id = account_user.account_id
            WHERE users_id = @userid
            ORDER BY name;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userid", user.Id),
        };

        using var reader = await _database.GetTable(query, queryParams);

        List<AccountEntity> res = [];
        foreach (DataRow row in reader.Rows)
        {
            res.Add(new AccountEntity(
                row.Field<int>("id"),
                row.Field<string>("name")!
            ));
        }

        return res;
    }
}
