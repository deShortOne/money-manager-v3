using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
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

    public async Task AddTransaction(TransactionEntity transaction)
    {
        var query = """
            INSERT INTO register (id, payee, amount, datePaid, category_id, account_id) VALUES
                (@id, @payee, @amount, @datePaid, @category_id, @account_id);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", transaction.Id),
            new NpgsqlParameter("payee", transaction.PayeeId),
            new NpgsqlParameter("amount", transaction.Amount),
            new NpgsqlParameter("datePaid", transaction.DatePaid),
            new NpgsqlParameter("category_id", transaction.CategoryId),
            new NpgsqlParameter("account_id", transaction.PayerId),
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
            setParamsLis.Add("payee = @payee");
            queryParams.Add(new NpgsqlParameter("payee", tramsaction.PayeeId));
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
            setParamsLis.Add("account_id = @account_id");
            queryParams.Add(new NpgsqlParameter("account_id", tramsaction.PayerId));
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

    public async Task<bool> IsTransactionOwnedByUser(AuthenticatedUser user, int transactionId)
    {
        var query = """
            SELECT 1
            FROM register
            WHERE id = @transaction_id
            AND account_id IN (
                SELECT id
                FROM account
                WHERE users_id = @user_id
            )
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("transaction_id", transactionId),
            new NpgsqlParameter("user_id", user.Id),
        };
        var reader = await _database.GetTable(query, queryParams);

        return reader.Rows.Count != 0 && reader.Rows[0].Field<int>(0) == 1;
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
