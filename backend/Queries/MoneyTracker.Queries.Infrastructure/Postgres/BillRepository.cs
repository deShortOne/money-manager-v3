using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class BillRepository : IBillDatabase
{
    private readonly IDatabase _database;
    public BillRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<ResultT<List<BillEntity>>> GetAllBills(AuthenticatedUser user)
    {
        string query = """
            SELECT b.id,
                payee_accounts.id as payee_id,
               	payee_accounts.name as payee_name,
               	amount,
               	nextduedate,
               	frequency,
               	c.id category_id,
                c.name category_name,
                b.monthday,
                accounts_owned_by_user.id payer_id,
                accounts_owned_by_user.name payer_name
            FROM bill b
            INNER JOIN category c
               	ON b.category_id = c.id
            INNER JOIN (
            	SELECT a.id, a.name
            	FROM account a
            	INNER JOIN users u
            		ON a.users_id = u.id 
            	WHERE u.id = @user_id
            ) accounts_owned_by_user
            	ON b.account_id = accounts_owned_by_user.id
            INNER JOIN account payee_accounts
                ON payee = payee_accounts.id
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
                row.Field<int>("payee_id")!,
                row.Field<string>("payee_name")!,
                row.Field<decimal>("amount"),
                DateOnly.FromDateTime(row.Field<DateTime>("nextduedate")),
                row.Field<int>("monthday"),
                row.Field<string>("frequency")!,
                row.Field<int>("category_id"),
                row.Field<string>("category_name")!,
                row.Field<int>("payer_id"),
                row.Field<string>("payer_name")!
            ));
        }

        return res;
    }
}
