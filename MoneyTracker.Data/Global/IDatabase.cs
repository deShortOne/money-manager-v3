
using System.Data.Common;

namespace MoneyTracker.Data.Global;

public interface IDatabase
{
    // will have to check if these are the correct super classes
    public Task<DbDataReader> GetTable(string query, List<DbParameter>? parameters = null);
    public Task<int> UpdateTable(string query, List<DbParameter>? parameters = null);
}
