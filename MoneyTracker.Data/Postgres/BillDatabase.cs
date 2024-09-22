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

    public async Task<List<BillEntityDTO>> AddBill(AuthenticatedUser user, NewBillDTO newBillDTO)
    {
        string query = """
            INSERT INTO bill (payee, amount, nextduedate, frequency, category_id, monthday, account_id)
            VALUES (@payee, @amount, @nextduedate, @frequency, @category_id, @monthday, @user_id);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("payee", newBillDTO.Payee),
                new NpgsqlParameter("amount", newBillDTO.Amount),
                new NpgsqlParameter("nextduedate", newBillDTO.NextDueDate),
                new NpgsqlParameter("frequency", newBillDTO.Frequency),
                new NpgsqlParameter("category_id", newBillDTO.Category),
                new NpgsqlParameter("monthday", newBillDTO.MonthDay),
                new NpgsqlParameter("user_id", user.UserId),
            };

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills(user);
    }

    public async Task<List<BillEntityDTO>> EditBill(AuthenticatedUser user, EditBillDTO editBillDTO)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
                new NpgsqlParameter("id", editBillDTO.Id),
                new NpgsqlParameter("user_id", user.UserId),
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
            setParamsLis.Add("monthday = @monthday");
            queryParams.Add(new NpgsqlParameter("monthday", ((DateOnly)editBillDTO.NextDueDate).Day));
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

        string query = $"""
            UPDATE bill
            SET {string.Join(",", setParamsLis)}
            WHERE id = @id
            AND bill.account_id IN (
                SELECT a.id
                FROM account a
                WHERE a.users_id = @user_id
            );
            """;

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills(user);
    }

    public async Task<List<BillEntityDTO>> DeleteBill(AuthenticatedUser user, DeleteBillDTO deleteBillDTO)
    {
        string query = """
            DELETE FROM bill
            WHERE id = @id
            AND account_id IN (
                SELECT id
                FROM account
                WHERE users_id = @user_id
            );
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", deleteBillDTO.Id),
                new NpgsqlParameter("user_id", user.UserId),
            };

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills(new AuthenticatedUser(1));
    }

    public async Task<BillEntityDTO> GetBillById(AuthenticatedUser user, int id)
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
            WHERE b.id = @id
            AND b.account_id IN (
                SELECT a.id
                FROM account
                WHERE a.users_id = @user_id
            );
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", id),
            new NpgsqlParameter("user_id", user.UserId),
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
}
