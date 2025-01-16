
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Transaction;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository;
public sealed class AddBudgetCategoryTest : RegisterRespositoryTestHelper
{
    [Fact]
    public async void AddBudgetItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        var id = 200;
        var payeeId = 3;
        var amount = 269.24m;
        var datePaid = new DateOnly(2024, 12, 1);
        var categoryId = 2;
        var payerId = 1;
        var newTransactionEntity = new TransactionEntity(id, payeeId, amount, datePaid, categoryId, payerId);

        await _registerRepo.AddTransaction(newTransactionEntity);

        List<TransactionEntity> results = await GetAllTransactionEntities();

        Assert.Multiple(() =>
        {
            Assert.Equal(11, results.Count);
            Assert.Equal(newTransactionEntity, results[0]);
        });
    }
}
