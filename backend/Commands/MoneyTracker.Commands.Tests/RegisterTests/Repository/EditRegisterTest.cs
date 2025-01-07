using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using Npgsql;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class EditRegisterTest : RegisterRespositoryTestHelper
{
    private readonly int _id = 1;
    private readonly int _payee = 7;
    private readonly decimal _amount = 90;
    private readonly DateOnly _datePaid = new DateOnly(2025, 5, 25);
    private readonly int _categoryId = 1;
    private readonly int _accountId = 2;

    private async Task SetupDb()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM register;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBudgetData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBudgetData.ExecuteNonQueryAsync();

        var addBaseTransactionData = """
            INSERT INTO register (id, payee, amount, datePaid, category_id, account_id) VALUES (@id, @payee, @amount, @datePaid, @category_id, @account_id);
            """;
        await using var commandAddBaseBillData = new NpgsqlCommand(addBaseTransactionData, conn);
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@id", _id));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@payee", _payee));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@amount", _amount));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@datePaid", _datePaid));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@category_id", _categoryId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@account_id", _accountId));
        await commandAddBaseBillData.ExecuteNonQueryAsync();
    }

    public static TheoryData<int?, int?, DateOnly?, int?, int?> OnlyOneItemNotNull = new() {
        { 5, null, null, null, null },
        { null, 500, null, null, null },
        { null, null, new DateOnly(2023, 2, 1), null, null },
        { null, null, null, 3, null },
        { null, null, null, null, 1 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBaseBillItemInDatabase(int? payee, int? amount, DateOnly? datePaid, int? categoryId, int? accountId)
    {
        await SetupDb();

        var editBillRequest = new EditTransactionEntity(_id, payee, amount, datePaid, categoryId, accountId);
        await _registerRepo.EditTransaction(editBillRequest);

        var expectedBillEntity = new TransactionEntity(_id,
            payee ?? _payee,
            amount ?? _amount,
            datePaid ?? _datePaid,
            categoryId ?? _categoryId,
            accountId ?? _accountId);

        var results = await GetAllTransactionEntities();

        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(expectedBillEntity, results[0]);
        });
    }
}
