
using System.Data;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Bill.Tests.Repository;
public sealed class EditBillTest : BillRespositoryTestHelper
{
    private readonly int _id = 456;
    private readonly string _payee = "DD";
    private readonly int _amount = 7;
    private readonly DateOnly _nextDueDate = new DateOnly(2024, 10, 23);
    private readonly string _frequency = "frequency";
    private readonly int _categoryId = 4;
    private readonly int _monthDay = 16;
    private readonly int _accountId = 2;

    private readonly BillEntity _baseEntity;
    public EditBillTest()
    {
        _baseEntity = new BillEntity(_id, _payee, _amount, _nextDueDate, _frequency, _categoryId, _monthDay, _accountId);
    }

    private async Task SetupDb()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM bill;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBillData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBillData.ExecuteNonQueryAsync();

        var addBaseBillData = """
            INSERT INTO bill (id, payee, amount, nextduedate, frequency, category_id, monthday, account_id) VALUES
            (@id, @payee, @amount, @nextDueDate, @frequency, @categoryId, @monthDay, @accountId);
            """;
        await using var commandAddBaseBillData = new NpgsqlCommand(addBaseBillData, conn);
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@id", _id));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@payee", _payee));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@amount", _amount));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@nextDueDate", _nextDueDate));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@frequency", _frequency));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@categoryId", _categoryId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@monthDay", _monthDay));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@accountId", _accountId));
        await commandAddBaseBillData.ExecuteNonQueryAsync();
    }

    private async Task<List<BillEntity>> GetAllBillEntity()
    {
        var getBillQuery = @"
                            SELECT id, payee, amount, nextduedate, frequency, category_id, monthday, account_id
                            FROM bill;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBillInfo = new NpgsqlCommand(getBillQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBillInfo.ExecuteReader();
        List<BillEntity> results = [];
        while (reader.Read())
        {
            results.Add(new BillEntity(id: reader.GetInt32("id"),
                payee: reader.GetString("payee"),
                amount: reader.GetDecimal("amount"),
                nextDueDate: DateOnly.FromDateTime(reader.GetDateTime("nextduedate")),
                frequency: reader.GetString("frequency"),
                category: reader.GetInt32("category_id"),
                monthDay: reader.GetInt32("monthday"),
                accountId: reader.GetInt32("account_id"))
            );
        }
        return results;
    }

    public static TheoryData<string?, decimal?, DateOnly?, string?, int?, int?> OnlyOneItemNotNull = new() {
        { "something funky here", null, null, null, null, null },
        { null, 245.23m, null, null, null, null },
        { null, null, new DateOnly(2023, 2, 21), null, null, null },
        { null, null, null, "Daily", null, null },
        { null, null, null, null, 5, null },
        { null, null, null, null, null, 1 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBaseBillItemInDatabase(string? payee,
        decimal? amount, DateOnly? nextDueDate, string? frequency, int? category, int? accountId)
    {
        await SetupDb();

        var editBillRequest = new EditBillEntity(_id, payee, amount, nextDueDate, frequency, category, accountId);
        await _billRepo.EditBill(editBillRequest);

        var expectedBillEntity = new BillEntity(_id,
            payee ?? _payee,
            amount ?? _amount,
            nextDueDate ?? _nextDueDate,
            frequency ?? _frequency,
            category ?? _categoryId,
            nextDueDate?.Day ?? _monthDay, // yuck
            accountId ?? _accountId);

        var results = await GetAllBillEntity();
        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(expectedBillEntity, results[0]);
        });
    }
}
