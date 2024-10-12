
using System.Data;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;
using Npgsql;

namespace MoneyTracker.Bill.Tests.Repository;

public sealed class DeleteBillTest : BillRespositoryTestHelper
{
    [Fact]
    public async void DeleteBillsFromSeed()
    {
        await _billRepo.DeleteBill(1);
        await _billRepo.DeleteBill(2);
        await _billRepo.DeleteBill(3);

        Assert.Multiple(async () =>
        {
            Assert.Empty(await GetAllBillEntity());
        });
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
}
