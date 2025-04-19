using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class RegisterDatabase : IRegisterDatabase
{
    private readonly IDatabase _database;

    public RegisterDatabase(IDatabase db)
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
                accPayer.id payer_id,
                accPayer.name payer_name
            FROM register
            INNER JOIN category c
                ON register.category_id = c.id
            INNER JOIN (
                SELECT account_user.id,
                    name,
                    users_id
                FROM account
                INNER JOIN account_user
                    ON account.id = account_user.account_id
                WHERE users_id = @user_id
                    AND user_owns_account
            ) accPayer
                ON accPayer.id = register.payer_user_id
            INNER JOIN (
                SELECT account_user.id,
                    name,
                    users_id
                FROM account
                INNER JOIN account_user
                    ON account.id = account_user.account_id
                WHERE users_id = @user_id
            ) accPayee
                ON accPayee.id = register.payee_user_id
            WHERE accPayer.users_id = @user_id
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
