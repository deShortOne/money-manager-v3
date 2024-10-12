using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Tests.BillTests.Repository;
public sealed class AddBillTest : BillRespositoryTestHelper
{
    [Fact]
    public async void AddBillItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM bill;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBillData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBillData.ExecuteNonQueryAsync();

        var id = 456;
        var payee = "DD";
        var amount = 7;
        var nextDueDate = new DateOnly(2024, 10, 23);
        var frequency = "frequency";
        var categoryId = 4;
        var monthDay = 16;
        var accountId = 2;
        var newBillEntity = new BillEntity(id, payee, amount, nextDueDate, frequency, categoryId, monthDay, accountId);


        await _billRepo.AddBill(newBillEntity);


        var getBillQuery = @"
                            SELECT id, payee, amount, nextduedate, frequency, category_id, monthday, account_id
                            FROM bill;
                            ";
        await using var commandGetBillInfo = new NpgsqlCommand(getBillQuery, conn);
        using var reader = commandGetBillInfo.ExecuteReader();
        List<BillEntity> results = await GetAllBillEntity();

        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(newBillEntity, results[0]);
        });
    }
}
