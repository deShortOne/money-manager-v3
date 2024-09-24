using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;
using Npgsql;

namespace MoneyTracker.Data.Postgres;

public class RegisterDatabase : IRegisterDatabase
{
    private readonly IDatabase _database;

    public RegisterDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<TransactionEntityDTO>> GetAllTransactions(AuthenticatedUser user)
    {
        var query = """
            SELECT register.id,
                   payee,
                   amount,
                   datePaid,
                   c.name category_name,
                   account.name account_name
            FROM register
            INNER JOIN category c
                ON register.category_id = c.id
            INNER JOIN account
            	ON account.id = register.account_id 
            WHERE account.users_id = @user_id
            ORDER BY datePaid DESC,
               	c.id ASC;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("user_id", user.UserId),
        };

        using var reader = await _database.GetTable(query, queryParams);

        var res = new List<TransactionEntityDTO>();
        while (await reader.ReadAsync())
        {
            res.Add(new TransactionEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("datePaid")),
                reader.GetString("category_name"),
                reader.GetString("account_name")
            ));
        }
        return res;
    }

    public async Task<TransactionEntityDTO> AddTransaction(NewTransactionDTO transaction)
    {
        var query = """
            INSERT INTO register (payee, amount, datePaid, category_id, account_id) VALUES
                (@payee, @amount, @datePaid, @category_id, @account_id)
            RETURNING (id),
                (payee), 
                (amount), 
                (datePaid), 
                (SELECT name AS category_name
                    FROM category
                    WHERE id = @category_id),
                (SELECT name AS account_name
                    FROM account
                    WHERE id = @account_id);
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("payee", transaction.Payee),
            new NpgsqlParameter("amount", transaction.Amount),
            new NpgsqlParameter("datePaid", transaction.DatePaid),
            new NpgsqlParameter("category_id", transaction.Category),
            new NpgsqlParameter("account_id", transaction.AccountId),
        };
        using var reader = await _database.GetTable(query, queryParams);
        if (await reader.ReadAsync())
        {
            return new TransactionEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("datePaid")),
                reader.GetString("category_name"),
                reader.GetString("account_name")
            );
        }
        throw new InvalidDataException("Database failed to return data");
    }

    public async Task<TransactionEntityDTO> EditTransaction(EditTransactionDTO tramsaction)
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
        if (tramsaction.Category != null)
        {
            setParamsLis.Add("category_id = @category_id");
            queryParams.Add(new NpgsqlParameter("category_id", tramsaction.Category));
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
                WHERE id = @id
                RETURNING (id),
                    (payee), 
                    (amount), 
                    (datePaid), 
                    (SELECT name category_name
                        FROM category
                        WHERE id = category_id),
                    (SELECT name account_id
                        FROM account
                        WHERE id = account_id);
                """;

        using var reader = await _database.GetTable(query, queryParams);
        if (await reader.ReadAsync())
        {
            return new TransactionEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("datePaid")),
                reader.GetString("category_name"),
                reader.GetString("account_id")
            );
        }
        throw new InvalidDataException("Database failed to return data");
    }

    public async Task<bool> DeleteTransaction(DeleteTransactionDTO transaction)
    {
        var query = """
            DELETE FROM register
            WHERE id = @id
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", transaction.Id),
        };
        var reader = await _database.UpdateTable(query, queryParams);
        return reader == 1;
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
            new NpgsqlParameter("user_id", user.UserId),
        };
        var reader = await _database.GetTable(query, queryParams);
        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0) == 1;
        }
        return false;
    }
}
