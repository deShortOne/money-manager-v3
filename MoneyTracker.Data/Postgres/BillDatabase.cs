using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Data.Postgres;
public class BillDatabase : IBillDatabase
{
    private readonly PostgresDatabase _database;
    public BillDatabase(IDatabase db)
    {
        _database = (PostgresDatabase)db;
    }

    public async Task<List<BillEntityDTO>> GetAllBills()
    {
        string query = """
            SELECT b.id,
            	payee,
            	amount,
            	nextduedate,
            	frequency,
            	c.name,
                b.monthday
            FROM bill b
            INNER JOIN category c
            	ON b.category_id = c.id
            ORDER BY nextduedate ASC;
            """;

        using var reader = await _database.GetTable(query);

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
                reader.GetInt32("monthday")
            ));
        }

        return res;
    }

    public async Task<List<BillEntityDTO>> AddBill(NewBillDTO newBillDTO)
    {
        // TODO - ACCOUNT ID
        string query = """
            INSERT INTO bill (payee, amount, nextduedate, frequency, category_id, monthday, account_id)
            VALUES (@payee, @amount, @nextduedate, @frequency, @category_id, @monthday, 1);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("payee", newBillDTO.Payee),
                new NpgsqlParameter("amount", newBillDTO.Amount),
                new NpgsqlParameter("nextduedate", newBillDTO.NextDueDate),
                new NpgsqlParameter("frequency", newBillDTO.Frequency),
                new NpgsqlParameter("category_id", newBillDTO.Category),
                new NpgsqlParameter("monthday", newBillDTO.MonthDay),
            };

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills();
    }

    public async Task<List<BillEntityDTO>> EditBill(EditBillDTO editBillDTO)
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
            WHERE id = @id;
            """;

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills();
    }

    public async Task<List<BillEntityDTO>> DeleteBill(DeleteBillDTO deleteBillDTO)
    {
        string query = """
            DELETE FROM bill
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", deleteBillDTO.Id),
            };

        await _database.UpdateTable(query, queryParams);

        return await GetAllBills();
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
                b.monthday
            FROM bill b
            INNER JOIN category c
            	ON b.category_id = c.id
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
                reader.GetInt32("monthday")
            );
        }

        throw new ArgumentException("Bill id was not found");
    }
}
