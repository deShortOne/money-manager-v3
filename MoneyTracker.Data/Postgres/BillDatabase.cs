using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Data.Postgres;
public class BillDatabase : IBillDatabase
{
    private readonly IDatabase _database;
    public BillDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<BillEntityDTO>> GetAllBills(AuthenticatedUser user)
    {
        string query = """
            SELECT b.id,
               	payee,
               	amount,
               	nextduedate,
               	frequency,
               	c.name,
                b.monthday,
                account.name account_name
            FROM bill b
            INNER JOIN category c
               	ON b.category_id = c.id
            INNER JOIN (
            	SELECT a.id, a.name
            	FROM account a
            	INNER JOIN users u
            		ON a.users_id = u.id 
            	WHERE u.id = @user_id
            ) account
            	ON b.account_id = account.id
            ORDER BY nextduedate ASC;
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("user_id", user.UserId),
            };

        using var reader = await _database.GetTable(query, queryParams);

        List<BillEntityDTO> res = [];
        while (await reader.ReadAsync())
        {
            var nextDueDate = DateOnly.FromDateTime(reader.GetDateTime("nextduedate"));
            var frequency = reader.GetString("frequency");

            res.Add(new BillEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                nextDueDate,
                frequency,
                reader.GetString("name"),
                reader.GetInt32("monthday"),
                reader.GetString("account_name")
            ));
        }

        return res;
    }

    public async Task AddBill(BillEntity newBillDTO)
    {
        string query = """
            INSERT INTO bill (id, payee, amount, nextduedate, frequency, category_id, monthday, account_id)
            VALUES (@id, @payee, @amount, @nextduedate, @frequency, @category_id, @monthday, @account_id);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", newBillDTO.Id),
                new NpgsqlParameter("payee", newBillDTO.Payee),
                new NpgsqlParameter("amount", newBillDTO.Amount),
                new NpgsqlParameter("nextduedate", newBillDTO.NextDueDate),
                new NpgsqlParameter("frequency", newBillDTO.Frequency),
                new NpgsqlParameter("category_id", newBillDTO.Category),
                new NpgsqlParameter("monthday", newBillDTO.MonthDay),
                new NpgsqlParameter("account_id", newBillDTO.AccountId),
            };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task EditBill(EditBillEntity editBillDTO)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
                new NpgsqlParameter("id", editBillDTO.Id),
        };

        if (editBillDTO.Payee != null)
        {
            setParamsLis.Add("payee = @payee");
            queryParams.Add(new NpgsqlParameter("payee", editBillDTO.Payee));
        }
        if (editBillDTO.Amount != null)
        {
            setParamsLis.Add("amount = @amount");
            queryParams.Add(new NpgsqlParameter("amount", editBillDTO.Amount));
        }
        if (editBillDTO.NextDueDate != null)
        {
            setParamsLis.Add("nextduedate = @nextduedate");
            queryParams.Add(new NpgsqlParameter("nextduedate", editBillDTO.NextDueDate));
        }
        if (editBillDTO.MonthDay != null)
        {
            setParamsLis.Add("monthday = @monthday");
            queryParams.Add(new NpgsqlParameter("monthday", editBillDTO.MonthDay));
        }
        if (editBillDTO.Frequency != null)
        {
            setParamsLis.Add("frequency = @frequency");
            queryParams.Add(new NpgsqlParameter("frequency", editBillDTO.Frequency));
        }
        if (editBillDTO.Category != null)
        {
            setParamsLis.Add("category_id = @category");
            queryParams.Add(new NpgsqlParameter("category", editBillDTO.Category));
        }
        if (editBillDTO.AccountId != null)
        {
            setParamsLis.Add("account_id = @account_id");
            queryParams.Add(new NpgsqlParameter("account_id", editBillDTO.AccountId));
        }

        string query = $"""
            UPDATE bill
            SET {string.Join(",", setParamsLis)}
            WHERE id = @id;
            """;

        await _database.UpdateTable(query, queryParams);
    }

    public async Task DeleteBill(int billIdToDelete)
    {
        string query = """
            DELETE FROM bill
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", billIdToDelete),
            };

        await _database.UpdateTable(query, queryParams);
    }

    public async Task<BillEntityDTO> GetBillById(int id)
    {
        string query = """
            SELECT b.id,
            	payee,
            	amount,
            	nextduedate,
            	frequency,
            	c.name,
                b.monthday,
                a.name account_name
            FROM bill b
            INNER JOIN category c
            	ON b.category_id = c.id
            INNER JOIN account a
                ON b.account_id = a.id
            WHERE b.id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", id),
        };
        using var reader = await _database.GetTable(query, queryParams);

        List<BillEntityDTO> res = [];
        if (await reader.ReadAsync())
        {
            var nextDueDate = DateOnly.FromDateTime(reader.GetDateTime("nextduedate"));
            var frequency = reader.GetString("frequency");

            return new BillEntityDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                nextDueDate,
                frequency,
                reader.GetString("name"),
                reader.GetInt32("monthday"),
                reader.GetString("account_name")
            );
        }

        throw new ArgumentException("Bill id was not found");
    }

    public async Task<bool> IsBillAssociatedWithUser(AuthenticatedUser user, int billId)
    {
        string query = """
            SELECT 1
            FROM bill b
            WHERE b.id = @id
            AND b.account_id IN (
                SELECT a.id
                FROM account a
                WHERE a.users_id = @user_id
            );
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", billId),
            new NpgsqlParameter("user_id", user.UserId),
        };
        using var reader = await _database.GetTable(query, queryParams);

        List<BillEntityDTO> res = [];
        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0) == 1;
        }
        return false;
    }

    public async Task<int> GetLastId()
    {
        string query = """
            SELECT max(id)
            FROM bill
            """;
        using var reader = await _database.GetTable(query);
        await reader.ReadAsync();

        return reader[0] == DBNull.Value ? 0 : reader.GetInt32(0); ;
    }
}
