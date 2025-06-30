
using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class ReceiptCommandRepository : IReceiptCommandRepository
{
    private readonly IDatabase _database;

    public ReceiptCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task AddReceipt(ReceiptEntity receipt)
    {
        var query = """
            INSERT INTO receipt_analysis_state (id, users_id, filename, url, state) VALUES
                (@id, @users_id, @filename, @url, @state);
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("id", receipt.Id),
            new NpgsqlParameter("users_id", receipt.UserId),
            new NpgsqlParameter("filename", receipt.Name),
            new NpgsqlParameter("url", receipt.Url),
            new NpgsqlParameter("state", receipt.State),
        };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task<ReceiptEntity?> GetReceiptById(string id)
    {
        var query = """
            SELECT id, users_id, filename, url, state
            FROM receipt_analysis_state
            WHERE id = @id;
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("id", id),
        };

        using var dataTable = await _database.GetTable(query, queryParams);
        if (dataTable.Rows.Count == 0)
            return null;

        var data = dataTable.Rows[0];
        return new ReceiptEntity(
            data.Field<string>("id")!,
            data.Field<int>("users_id"),
            data.Field<string>("filename")!,
            data.Field<string>("url")!,
            data.Field<int>("state"));
    }

    public async Task UpdateReceipt(ReceiptEntity receipt)
    {
        var query = """
            UPDATE receipt_analysis_state
                SET users_id = @users_id,
                    filename = @filename,
                    url = @url,
                    state = @state
                WHERE id = @id;
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("id", receipt.Id),
            new NpgsqlParameter("users_id", receipt.UserId),
            new NpgsqlParameter("filename", receipt.Name),
            new NpgsqlParameter("url", receipt.Url),
            new NpgsqlParameter("state", receipt.State),
        };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task CreateTemporaryTransaction(int userId, TemporaryTransactionEntity temporaryTransactionEntity)
    {
        var query = """
            INSERT INTO receipt_to_register (users_id, payee, amount, datepaid, category_id, account_id) VALUES
                (@users_id, @payee, @amount, @datepaid, @category_id, @account_id);
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("users_id", userId),
            new NpgsqlParameter<int?>("payee", temporaryTransactionEntity.PayeeId),
            new NpgsqlParameter<decimal?>("amount", temporaryTransactionEntity.Amount),
            new NpgsqlParameter<DateOnly?>("datepaid", temporaryTransactionEntity.DatePaid),
            new NpgsqlParameter<int?>("category_id", temporaryTransactionEntity.CategoryId),
            new NpgsqlParameter<int?>("account_id", temporaryTransactionEntity.PayerId),
        };

        await _database.UpdateTable(query, queryParams);
    }
}
