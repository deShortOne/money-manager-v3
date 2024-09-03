
using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Shared.Models.Budget;
using Npgsql;

namespace MoneyTracker.Data.Postgres;
public class BillDatabase : IBillDatabase
{
    private readonly PostgresDatabase _database;
    public BillDatabase(IDatabase db)
    {
        _database = (PostgresDatabase)db;
    }

    public async Task<List<BillDTO>> GetBill()
    {
        string query = """
            SELECT b.id,
            	payee,
            	amount,
            	nextduedate,
            	frequency,
            	c.name
            FROM bill b
            INNER JOIN category c
            	ON b.categoryid = c.id
            ORDER BY nextduedate ASC;
            """;

        using var reader = await _database.GetTable(query);

        List<BillDTO> res = [];
        while (await reader.ReadAsync())
        {
            res.Add(new BillDTO(
                reader.GetInt32("id"),
                reader.GetString("payee"),
                reader.GetDecimal("amount"),
                DateOnly.FromDateTime(reader.GetDateTime("nextduedate")),
                reader.GetString("frequency"),
                reader.GetString("name"))
            );
        }

        return res;
    }

    public async Task<List<BillDTO>> AddBill(NewBillDTO newBillDTO)
    {
        string query = """
            INSERT INTO bill (payee, amount, nextduedate, frequency, categoryid)
            VALUES (@payee, @amount, @nextduedate, @frequency, @categoryid);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("payee", newBillDTO.Payee),
                new NpgsqlParameter("amount", newBillDTO.Amount),
                new NpgsqlParameter("nextduedate", newBillDTO.NextDueDate),
                new NpgsqlParameter("frequency", newBillDTO.Frequency),
                new NpgsqlParameter("categoryid", newBillDTO.Category),
            };

        await _database.UpdateTable(query, queryParams);

        return await GetBill();
    }

    public async Task<List<BillDTO>> EditBill(EditBillDTO editBillDTO)
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
        if (editBillDTO.Frequency != null)
        {
            setParamsLis.Add("frequency = @frequency");
            queryParams.Add(new NpgsqlParameter("frequency", editBillDTO.Frequency));
        }
        if (editBillDTO.Category != null)
        {
            setParamsLis.Add("categoryid = @category");
            queryParams.Add(new NpgsqlParameter("category", editBillDTO.Category));
        }

        string query = $"""
            UPDATE bill
            SET {string.Join(",", setParamsLis)}
            WHERE id = @id;
            """;

        await _database.UpdateTable(query, queryParams);

        return await GetBill();
    }

    public async Task<List<BillDTO>> DeleteBill(DeleteBillDTO deleteBillDTO)
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

        return await GetBill();
    }
}
