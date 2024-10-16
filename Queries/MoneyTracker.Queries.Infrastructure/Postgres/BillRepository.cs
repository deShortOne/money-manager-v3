using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class BillRepository : IBillRepository
{
    private readonly IDatabase _database;
    public BillRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<BillEntity>> GetAllBills(AuthenticatedUser user)
    {
        string query = """
            SELECT b.id,
               	payee,
               	amount,
               	nextduedate,
               	frequency,
               	c.name,
                b.monthday,
                account.name account_name
            FROM bill b
            INNER JOIN category c
               	ON b.category_id = c.id
            INNER JOIN (
            	SELECT a.id, a.name
            	FROM account a
            	INNER JOIN users u
            		ON a.users_id = u.id 
            	WHERE u.id = @user_id
            ) account
            	ON b.account_id = account.id
            ORDER BY nextduedate ASC;
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("user_id", user.Id),
            };

        using var reader = await _database.GetTable(query, queryParams);

        List<BillEntity> res = [];
        while (await reader.ReadAsync())
        {
            res.Add(new BillEntity(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("nextduedate")),
                reader.GetInt32("monthday"),
                reader.GetString("frequency"),
                reader.GetString("name"),
                reader.GetString("account_name")
            ));
        }

        return res;
    }
}
