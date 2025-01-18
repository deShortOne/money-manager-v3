using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class RegisterRepository : IRegisterRepository
{
    private readonly IDatabase _database;

    public RegisterRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user)
    {
        var query = """
			SELECT register.id,
			    accPayee.id payee_id,
				accPayee.name payee_name,
				amount,
				datePaid,
				c.id category_id,
				c.name category_name,
				accAcc.id payer_id,
				accAcc.name payer_name
			FROM register
			INNER JOIN category c
				ON register.category_id = c.id
			INNER JOIN account accAcc
				ON accAcc.id = register.account_id
			INNER JOIN account accPayee
				ON accPayee.id = register.payee
			WHERE accAcc.users_id = @user_id
			ORDER BY datePaid DESC,
				c.id ASC;
			""";
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("user_id", user.Id),
        };

        using var reader = await _database.GetTable(query, queryParams);

        var res = new List<TransactionEntity>();
        foreach (DataRow row in reader.Rows)
        {
            res.Add(new TransactionEntity(
                row.Field<int>("id"),
                row.Field<int>("payee_id"),
                row.Field<string>("payee_name")!,
                row.Field<decimal>("amount"),
                DateOnly.FromDateTime(row.Field<DateTime>("datePaid")),
                row.Field<int>("category_id")!,
                row.Field<string>("category_name")!,
                row.Field<int>("payer_id")!,
                row.Field<string>("payer_name")!
            ));
        }
        return res;
    }
}
