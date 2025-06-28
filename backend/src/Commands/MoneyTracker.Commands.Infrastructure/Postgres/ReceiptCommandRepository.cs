
using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class ReceiptCommandRepository : IReceiptCommandRepository
{
    private readonly IDatabase _database;

    public ReceiptCommandRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task AddReceipt(ReceiptEntity receipt)
    {
        var query = """
            INSERT INTO receipt_analysis_state (id, users_id, filename, url, state) VALUES
                (@id, @users_id, @filename, @url, @state);
        """;
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("id", receipt.Id),
            new NpgsqlParameter("users_id", receipt.UserId),
            new NpgsqlParameter("filename", receipt.Name),
            new NpgsqlParameter("url", receipt.Url),
            new NpgsqlParameter("state", receipt.State),
        };

        await _database.UpdateTable(query, queryParams);
    }
}
