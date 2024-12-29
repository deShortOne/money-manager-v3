using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Entities.Account;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class AccountRepository : IAccountRepository
{
    private readonly IDatabase _database;
    public AccountRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<AccountEntity>> GetAccounts(AuthenticatedUser user)
    {
        var query = """
            SELECT id, name
            FROM account
            WHERE users_id = @userid
            ORDER BY name;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userid", user.Id),
        };

        var reader = await _database.GetTable(query, queryParams);

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
