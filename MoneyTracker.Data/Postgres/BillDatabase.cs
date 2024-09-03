
using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Bill;
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
}
