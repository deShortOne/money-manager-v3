using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
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
                new NpgsqlParameter("category_id", newBillDTO.CategoryId),
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
        if (editBillDTO.CategoryId != null)
        {
            setParamsLis.Add("category_id = @category");
            queryParams.Add(new NpgsqlParameter("category", editBillDTO.CategoryId));
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

    public async Task<BillEntity?> GetBillById(int id)
    {
        string query = """
            SELECT id,
            	payee,
            	amount,
            	nextduedate,
            	frequency,
            	category_id,
                monthday,
                account_id
            FROM bill b
            WHERE id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", id),
        };
        using var reader = await _database.GetTable(query, queryParams);

        if (reader.Rows.Count != 0)
        {
            var currRow = reader.Rows[0];

            return new BillEntity(
                currRow.Field<int>("id"),
                currRow.Field<string>("payee")!,
                currRow.Field<decimal>("amount"),
                DateOnly.FromDateTime(currRow.Field<DateTime>("nextduedate")),
                currRow.Field<int>("monthday"),
                currRow.Field<string>("frequency")!,
                currRow.Field<int>("category_id"),
                currRow.Field<int>("account_id")
            );
        }

        return null;
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
            new NpgsqlParameter("user_id", user.Id),
        };
        using var reader = await _database.GetTable(query, queryParams);

        return reader.Rows.Count != 0 && reader.Rows[0].Field<int>(0) == 1;
    }

    public async Task<int> GetLastId()
    {
        string query = """
            SELECT max(id) as last_id
            FROM bill;
        """;
        using var reader = await _database.GetTable(query);

        var a = reader.Rows.Count;
        var b = reader.Rows[0];

        var returnDefaultValue = reader.Rows.Count == 0 || reader.Rows[0].ItemArray[0] == DBNull.Value;
        return  returnDefaultValue ? 0 : reader.Rows[0].Field<int>(0);
    }
}
