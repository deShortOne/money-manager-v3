using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Bill;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class BillCommandRepository : IBillCommandRepository
{
    private readonly IDatabase _database;
    public BillCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task AddBill(BillEntity newBillDTO, CancellationToken cancellationToken)
    {
        string query = """
            INSERT INTO bill (id, payee_user_id, amount, nextduedate, frequency, category_id, monthday, payer_user_id)
            VALUES (@id, @payee_user_id, @amount, @nextduedate, @frequency, @category_id, @monthday, @payer_user_id);
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", newBillDTO.Id),
                new NpgsqlParameter("payee_user_id", newBillDTO.PayeeId),
                new NpgsqlParameter("amount", newBillDTO.Amount),
                new NpgsqlParameter("nextduedate", newBillDTO.NextDueDate),
                new NpgsqlParameter("frequency", newBillDTO.Frequency),
                new NpgsqlParameter("category_id", newBillDTO.CategoryId),
                new NpgsqlParameter("monthday", newBillDTO.MonthDay),
                new NpgsqlParameter("payer_user_id", newBillDTO.PayerId),
            };

        await _database.UpdateTable(query, cancellationToken, queryParams);
    }

    public async Task EditBill(EditBillEntity editBillDTO, CancellationToken cancellationToken)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
                new NpgsqlParameter("id", editBillDTO.Id),
        };

        if (editBillDTO.PayeeId != null)
        {
            setParamsLis.Add("payee_user_id = @payee_user_id");
            queryParams.Add(new NpgsqlParameter("payee_user_id", editBillDTO.PayeeId));
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
        if (editBillDTO.CategoryId != null)
        {
            setParamsLis.Add("category_id = @category");
            queryParams.Add(new NpgsqlParameter("category", editBillDTO.CategoryId));
        }
        if (editBillDTO.PayerId != null)
        {
            setParamsLis.Add("payer_user_id = @payer_user_id");
            queryParams.Add(new NpgsqlParameter("payer_user_id", editBillDTO.PayerId));
        }

        string query = $"""
            UPDATE bill
            SET {string.Join(",", setParamsLis)}
            WHERE id = @id;
            """;

        await _database.UpdateTable(query, cancellationToken, queryParams);
    }

    public async Task DeleteBill(int billIdToDelete, CancellationToken cancellationToken)
    {
        string query = """
            DELETE FROM bill
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
            {
                new NpgsqlParameter("id", billIdToDelete),
            };

        await _database.UpdateTable(query, cancellationToken, queryParams);
    }

    public async Task<BillEntity?> GetBillById(int id, CancellationToken cancellationToken)
    {
        string query = """
            SELECT id,
            	payee_user_id,
            	amount,
            	nextduedate,
            	frequency,
            	category_id,
                monthday,
                payer_user_id
            FROM bill b
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", id),
        };
        using var reader = await _database.GetTable(query, cancellationToken, queryParams);

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];

            return new BillEntity(
                currRow.Field<int>("id"),
                currRow.Field<int>("payee_user_id"),
                currRow.Field<decimal>("amount"),
                DateOnly.FromDateTime(currRow.Field<DateTime>("nextduedate")),
                currRow.Field<int>("monthday"),
                currRow.Field<string>("frequency")!,
                currRow.Field<int>("category_id"),
                currRow.Field<int>("payer_user_id")
            );
        }

        return null;
    }

    public async Task<int> GetLastId(CancellationToken cancellationToken)
    {
        string query = """
            SELECT max(id) as last_id
            FROM bill;
        """;
        using var reader = await _database.GetTable(query, cancellationToken);

        var returnDefaultValue = reader.Rows.Count == 0 || reader.Rows[0].ItemArray[0] == DBNull.Value;
        return returnDefaultValue ? 0 : reader.Rows[0].Field<int>(0);
    }
}
