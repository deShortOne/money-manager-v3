
using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Account;
using Npgsql;

namespace MoneyTracker.Data.Postgres;

public class AccountDatabase : IAccountDatabase
{
    private readonly IDatabase _database;
    public AccountDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<AccountEntityDTO>> GetAccounts(AuthenticatedUser user)
    {
        var query = """
            SELECT id, name
            FROM account
            WHERE users_id = @userid
            ORDER BY name;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userid", user.UserId),
        };

        var reader = await _database.GetTable(query, queryParams);

        List<AccountEntityDTO> res = [];
        while (await reader.ReadAsync())
        {
            res.Add(new AccountEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("name")
            ));
        }

        return res;
    }
}
