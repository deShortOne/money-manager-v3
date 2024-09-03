
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Data.Postgres;
public class BillDatabase : IBillDatabase
{
    private readonly PostgresDatabase _database;
    public BillDatabase(IDatabase db)
    {
        _database = (PostgresDatabase)db;
    }

    public Task<List<BillDTO>> GetBill()
    {
        return null;
    }
}
