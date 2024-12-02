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
            new NpgsqlParameter("payee", transaction.Payee),
            new NpgsqlParameter("amount", transaction.Amount),
            new NpgsqlParameter("datePaid", transaction.DatePaid),
            new NpgsqlParameter("category_id", transaction.CategoryId),
            new NpgsqlParameter("account_id", transaction.AccountId),
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
        if (tramsaction.Payee != null)
        {
            setParamsLis.Add("payee = @payee");
            queryParams.Add(new NpgsqlParameter("payee", tramsaction.Payee));
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
        if (tramsaction.AccountId != null)
        {
            setParamsLis.Add("account_id = @account_id");
            queryParams.Add(new NpgsqlParameter("account_id", tramsaction.AccountId));
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
        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0) == 1;
        }
        return false;
    }
}
