using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Database;
using Npgsql;
using NpgsqlTypes;

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
                user_payee."name" payee_name,
                amount,
                datepaid,
                category_id,
                category."name" category_name,
                account_id,
                user_payer."name" payer_name
            FROM receipt_to_register
            LEFT JOIN users user_payee
            ON user_payee.id = payee
            LEFT JOIN users user_payer
            ON user_payer.id = account_id
            LEFT JOIN category
            ON category.id = category_id 
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

        var payee = data.Field<int?>("payee") == null
            ? null
            : new AccountResponse(data.Field<int>("payee"), data.Field<string>("payee_name")!);

        var category = data.Field<int?>("category_id") == null
            ? null
            : new CategoryResponse(data.Field<int>("category_id"), data.Field<string>("category_name")!);

        var payer = data.Field<int?>("account_id") == null
            ? null
            : new AccountResponse(data.Field<int>("account_id"), data.Field<string>("payer_name")!);

        DateOnly? datePaid = data.Field<DateTime?>("datepaid") == null
            ? null
            : DateOnly.FromDateTime(data.Field<DateTime>("datepaid"));

        return new TemporaryTransaction(
            data.Field<int>("users_id"),
            data.Field<string>("filename")!,
            payee,
            data.Field<decimal?>("amount"),
            datePaid,
            category,
            payer
        );
    }

    public async Task<List<ReceiptIdAndStateEntity>> GetReceiptStatesForUser(AuthenticatedUser user, List<int> designatedStates, CancellationToken cancellationToken)
    {
        var query = """
            SELECT id,
                state
            FROM receipt_analysis_state
            WHERE users_id = @users_id
                AND state = ANY(@states);
        """;

        var lis = new NpgsqlParameter("states", NpgsqlDbType.Array | NpgsqlDbType.Integer);
        lis.Value = designatedStates.ToArray();
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("users_id", user.Id),
            lis,
        };

        using var dataTable = await _database.GetTable(query, cancellationToken, queryParams);

        return dataTable.Rows
            .OfType<DataRow>()
            .Select(x => new ReceiptIdAndStateEntity(x.Field<string>("id")!, x.Field<int>("state")))
            .ToList();
    }
}
