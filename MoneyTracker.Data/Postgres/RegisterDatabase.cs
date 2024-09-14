using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;
using Npgsql;

namespace MoneyTracker.Data.Postgres
{
    public class RegisterDatabase : IRegisterDatabase
    {
        private readonly PostgresDatabase _database;

        public RegisterDatabase(IDatabase db)
        {
            _database = (PostgresDatabase)db;
        }

        public async Task<List<TransactionEntityDTO>> GetAllTransactions()
        {
            var query = """
                SELECT register.id,
                       payee,
                       amount,
                       datePaid,
                       c.name
                FROM register
                INNER JOIN category c
                    	ON register.category_id = c.id
                ORDER BY datePaid DESC,
                	c.id ASC;
                """;

            // get category id
            using var reader = await _database.GetTable(query);

            var res = new List<TransactionEntityDTO>();
            while (await reader.ReadAsync())
            {
                res.Add(new TransactionEntityDTO(
                    reader.GetInt32("id"),
                    reader.GetString("payee"),
                    reader.GetDecimal("amount"),
                    reader.GetDateTime("datePaid"),
                    reader.GetString("name")
                ));
            }
            return res;
        }

        public async Task<TransactionEntityDTO> AddTransaction(NewTransactionDTO transaction)
        {
            // TODO - ACCOUNT ID
            var query = """
                INSERT INTO register (payee, amount, datePaid, category_id, account_id) VALUES
                    (@payee, @amount, @datePaid, @category_id, 1)
                RETURNING (id),
                    (payee), 
                    (amount), 
                    (datePaid), 
                    (SELECT name
                    FROM category
                    WHERE id = @category_id);
                """;
            var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("payee", transaction.Payee),
                new NpgsqlParameter("amount", transaction.Amount),
                new NpgsqlParameter("datePaid", transaction.DatePaid),
                new NpgsqlParameter("category_id", transaction.Category),
            };
            using var reader = await _database.GetTable(query, queryParams);
            if (await reader.ReadAsync())
            {
                return new TransactionEntityDTO(
                    reader.GetInt32("id"),
                    reader.GetString("payee"),
                    reader.GetDecimal("amount"),
                    reader.GetDateTime("datePaid"),
                    reader.GetString("name")
                );
            }
            throw new ExternalException("Database failed to return data");
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
                    (SELECT name
                    FROM category
                    WHERE id = @category_id);
                """;

            using var reader = await _database.GetTable(query, queryParams);
            if (await reader.ReadAsync())
            {
                return new TransactionEntityDTO(
                    reader.GetInt32("id"),
                    reader.GetString("payee"),
                    reader.GetDecimal("amount"),
                    reader.GetDateTime("datePaid"),
                    reader.GetString("name")
                );
            }
            throw new ExternalException("Database failed to return data");
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
    }
}
