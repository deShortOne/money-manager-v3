using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Bill;
using Npgsql;

namespace MoneyTracker.Commands.Tests.BillTests.Repository;
public sealed class AddBillTest : BillRespositoryTestHelper
{
    [Fact]
    public async Task AddBillItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM bill;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBillData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBillData.ExecuteNonQueryAsync();

        var id = 456;
        var payee = 6;
        var amount = 7;
        var nextDueDate = new DateOnly(2024, 10, 23);
        var frequency = "frequency";
        var categoryId = 4;
        var monthDay = 16;
        var payerId = 2;
        var newBillEntity = new BillEntity(id, payee, amount, nextDueDate, monthDay, frequency, categoryId, payerId);

        await _billRepo.AddBill(newBillEntity, CancellationToken.None);

        List<BillEntity> results = await GetAllBillEntity();

        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(newBillEntity, results[0]);
        });
    }
}
