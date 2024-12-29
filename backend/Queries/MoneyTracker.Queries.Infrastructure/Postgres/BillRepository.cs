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
        foreach (DataRow row in reader.Rows)
        {
            res.Add(new BillEntity(
                row.Field<int>("id"),
                row.Field<string>("payee")!,
                row.Field<decimal>("amount"),
                DateOnly.FromDateTime(row.Field<DateTime>("nextduedate")),
                row.Field<int>("monthday"),
                row.Field<string>("frequency")!,
                row.Field<string>("name")!,
                row.Field<string>("account_name")!
            ));
        }

        return res;
    }
}
