using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class RegisterCommandRepository : IRegisterCommandRepository
{
    private readonly IDatabase _database;

    public RegisterCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<TransactionEntity?> GetTransaction(int transactionId)
    {
        var query = """
            SELECT id,
                payee_user_id,
                amount,
                datepaid,
                category_id,
                payer_user_id
            FROM register
            WHERE id = @transaction_id;
         """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("transaction_id", transactionId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
            return new TransactionEntity(
                reader.Rows[0].Field<int>("id"),
                reader.Rows[0].Field<int>("payee_user_id"),
                reader.Rows[0].Field<decimal>("amount"),
                DateOnly.FromDateTime(reader.Rows[0].Field<DateTime>("datepaid")),
                reader.Rows[0].Field<int>("category_id"),
                reader.Rows[0].Field<int>("payer_user_id"));
        return null;
    }

    public async Task AddTransaction(TransactionEntity transaction)
    {
        var query = """
            INSERT INTO register (id, payee_user_id, amount, datePaid, category_id, payer_user_id) VALUES
                (@id, @payee_user_id, @amount, @datePaid, @category_id, @payer_user_id);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", transaction.Id),
            new NpgsqlParameter("payee_user_id", transaction.PayeeId),
            new NpgsqlParameter("amount", transaction.Amount),
            new NpgsqlParameter("datePaid", transaction.DatePaid),
            new NpgsqlParameter("category_id", transaction.CategoryId),
            new NpgsqlParameter("payer_user_id", transaction.PayerId),
        };

        await _database.GetTable(query, queryParams);
    }

    public async Task EditTransaction(EditTransactionEntity tramsaction)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", tramsaction.Id),
        };
        if (tramsaction.PayeeId != null)
        {
            setParamsLis.Add("payee_user_id = @payee_user_id");
            queryParams.Add(new NpgsqlParameter("payee_user_id", tramsaction.PayeeId));
        }
        if (tramsaction.Amount != null)
        {
            setParamsLis.Add("amount = @amount");
            queryParams.Add(new NpgsqlParameter("amount", tramsaction.Amount));
        }
        if (tramsaction.DatePaid != null)
        {
            setParamsLis.Add("datePaid = @datePaid");
            queryParams.Add(new NpgsqlParameter("datePaid", tramsaction.DatePaid));
        }
        if (tramsaction.CategoryId != null)
        {
            setParamsLis.Add("category_id = @category_id");
            queryParams.Add(new NpgsqlParameter("category_id", tramsaction.CategoryId));
        }
        if (tramsaction.PayerId != null)
        {
            setParamsLis.Add("payer_user_id = @payer_user_id");
            queryParams.Add(new NpgsqlParameter("payer_user_id", tramsaction.PayerId));
        }

        if (setParamsLis.Count == 0)
        {
            throw new ArgumentException("Value to update must exist");
        }

        var query = $"""
            UPDATE register 
                SET {string.Join(",", setParamsLis)}
            WHERE id = @id;
            """;

        await _database.GetTable(query, queryParams);
    }

    public async Task DeleteTransaction(int transactionId)
    {
        var query = """
            DELETE FROM register
            WHERE id = @id
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", transactionId),
        };
        await _database.UpdateTable(query, queryParams);
    }

    public async Task<int> GetLastTransactionId()
    {
        var query = """
            SELECT MAX(id) AS last_id
            FROM register;
        """;

        var reader = await _database.GetTable(query);

        if (reader.Rows.Count != 0)
        {
            return reader.Rows[0].Field<int>("last_id");
        }
        return 0;
    }
}
