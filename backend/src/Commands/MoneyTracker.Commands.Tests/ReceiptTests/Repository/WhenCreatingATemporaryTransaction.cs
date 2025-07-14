
using System.Data;
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Npgsql;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public class WhenCreatingATemporaryTransaction : ReceiptTestHelper
{
    public static TheoryData<int?, decimal?, DateOnly?, int?, int?> OnlyOneItemNotNull = new()
    {
        {null, null, null, null, null },
        {1, null, null, null, null },
        {null, 23m, null, null, null },
        {null, null, new DateOnly(2025, 7, 14), null, null },
        {null, null, null, 5, null },
        {null, null, null, null, 8 },
        {1, 23m, new DateOnly(2025, 7, 14), 5, 8 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async Task SuccessfullyEditBill_OnlyChangeOneItem(int? payeeId, decimal? amount, DateOnly? datePaid, int? categoryId, int? payerId)
    {
        var userId = 1;
        var filename = "fdas";
        var entity = new TemporaryTransactionEntity
        {
            UserId = userId,
            Filename = filename,
            PayeeId = payeeId,
            Amount = amount,
            DatePaid = datePaid,
            CategoryId = categoryId,
            PayerId = payerId,
        };
        await _receiptRepo.CreateTemporaryTransaction(entity, CancellationToken.None);

        var resultEntity = await GetTemporaryTransactionEntity(userId, filename);

        Assert.NotNull(resultEntity);
        Assert.Equal(userId, resultEntity.UserId);
        Assert.Equal(filename, resultEntity.Filename);
        Assert.Equal(payeeId, resultEntity.PayeeId);
        Assert.Equal(amount, resultEntity.Amount);
        Assert.Equal(datePaid, resultEntity.DatePaid);
        Assert.Equal(categoryId, resultEntity.CategoryId);
        Assert.Equal(payerId, resultEntity.PayerId);
    }

    private async Task<TemporaryTransactionEntity?> GetTemporaryTransactionEntity(int userId, string filename)
    {
        var query = """
            SELECT users_id,
                filename,
                payee,
                amount,
                datepaid,
                category_id,
                account_id
            FROM receipt_to_register
            WHERE filename = @fileId;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("fileId", filename),
        };

        using var reader = await _database.GetTable(query, CancellationToken.None, queryParams);

        if (reader.Rows.Count == 0)
            return null;

        var data = reader.Rows[0];

        DateOnly? datePaid = null;
        var datePaidDb = data.Field<DateTime?>("datepaid");
        if (datePaidDb is not null)
            datePaid = DateOnly.FromDateTime((DateTime)datePaidDb!);
        return new TemporaryTransactionEntity
        {
            UserId = data.Field<int>("users_id"),
            Filename = data.Field<string>("filename"),
            PayeeId = data.Field<int?>("payee"),
            Amount = data.Field<decimal?>("amount"),
            DatePaid = datePaid,
            CategoryId = data.Field<int?>("category_id"),
            PayerId = data.Field<int?>("account_id"),
        };
    }
}
