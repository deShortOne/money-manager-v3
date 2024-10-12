
using System.Data;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Bill.Tests.Repository;
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
        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(newBillEntity, results[0]);
        });
    }
}
