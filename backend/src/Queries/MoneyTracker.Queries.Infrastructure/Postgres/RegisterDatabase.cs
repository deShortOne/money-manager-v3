using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Queries.Domain.Entities.Receipt;
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

    public async Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken)
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

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

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

    public async Task<ResultT<ReceiptEntity>> GetReceiptProcessingInfo(string fileId, CancellationToken cancellationToken)
    {
        var query = """
            select id,
                users_id,
                filename,
                url,
                state
            FROM receipt_analysis_state
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", fileId),
        };

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count == 0)
            return Error.NotFound("", $"Could not find receipt procesing information for given id: {fileId}");

        var data = reader.Rows[0];

        return new ReceiptEntity(
            data.Field<string>("id")!,
            data.Field<int>("users_id"),
            data.Field<string>("filename")!,
            data.Field<string>("url")!,
            (ReceiptState)data.Field<int>("state")
        );
    }

    public async Task<ResultT<TemporaryTransaction>> GetTemporaryTransactionFromReceipt(string fileId, CancellationToken cancellationToken)
    {
        var query = """
            SELECT users_id,
                filename,
                payee,
                amount,
                datepaid,
                category_id,
                account_id
            FROM receipt_to_register
            WHERE filename = @fileId;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("fileId", fileId),
        };

        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count == 0)
            return Error.NotFound("", $"Could not find temporary transaction information for given id: {fileId}");

        var data = reader.Rows[0];

        return new TemporaryTransaction(
            data.Field<int>("users_id"),
            data.Field<string>("filename")!,
            null,
            data.Field<decimal?>("amount"),
            null,
            null,
            null
        );
    }
}
