using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Bill;
using Npgsql;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class EditBillTest : BillRespositoryTestHelper
{
    private readonly int _id = 456;
    private readonly int _payeeId = 3;
    private readonly int _amount = 7;
    private readonly DateOnly _nextDueDate = new DateOnly(2024, 10, 23);
    private readonly string _frequency = "frequency";
    private readonly int _categoryId = 4;
    private readonly int _monthDay = 16;
    private readonly int _payerId = 2;

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
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@payee", _payeeId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@amount", _amount));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@nextDueDate", _nextDueDate));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@frequency", _frequency));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@categoryId", _categoryId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@monthDay", _monthDay));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@accountId", _payerId));
        await commandAddBaseBillData.ExecuteNonQueryAsync();
    }

    public static TheoryData<int?, decimal?, DateOnly?, int?, string, int?, int?> OnlyOneItemNotNull = new() {
        { 5, null, null, null, null, null, null },
        { null, 245.23m,  null, null, null, null, null },
        { null, null, new DateOnly(2023, 2, 21), null, null, null, null },
        { null, null, null, 1, null, null, null },
        { null, null, null, null, "Daily", null, null },
        { null, null, null, null, null, 5, null },
        { null, null, null, null, null, null, 1 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBaseBillItemInDatabase(int? payee,
        decimal? amount, DateOnly? nextDueDate, int? monthDay, string frequency, int? category, int? accountId)
    {
        await SetupDb();

        var editBillRequest = new EditBillEntity(_id, payee, amount, nextDueDate, monthDay, frequency, category, accountId);
        await _billRepo.EditBill(editBillRequest);

        var expectedBillEntity = new BillEntity(_id,
            payee ?? _payeeId,
            amount ?? _amount,
            nextDueDate ?? _nextDueDate,
            monthDay ?? _monthDay,
            frequency ?? _frequency,
            category ?? _categoryId,
            accountId ?? _payerId);

        var results = await GetAllBillEntity();
        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(expectedBillEntity, results[0]);
        });
    }
}
