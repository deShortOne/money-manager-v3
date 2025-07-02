using System.Data;
using System.Data.Common;

namespace MoneyTracker.Common.Interfaces;
public interface IDatabase
{
    // will have to check if these are the correct super classes
    public Task<DataTable> GetTable(string query,
        CancellationToken cancellationToken, List<DbParameter>? parameters = null);
    public Task<int> UpdateTable(string query, CancellationToken cancellationToken,
        List<DbParameter>? parameters = null);
}
