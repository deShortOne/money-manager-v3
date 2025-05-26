using System.Data;
using System.Data.Common;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class PostgresDatabase : IDatabase
{
    private readonly NpgsqlDataSource _dataSource_ro;

    // TODO: have ro string
    public PostgresDatabase(string connectionString)
    {
        var dataSourceBuilder_ro = new NpgsqlDataSourceBuilder(connectionString);
        _dataSource_ro = dataSourceBuilder_ro.Build();
    }

    public async Task<DataTable> GetTable(string query, List<DbParameter>? parameters = null)
    {
        await using (var conn = await _dataSource_ro.OpenConnectionAsync())
        {
            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                var dataTable = new DataTable();
                dataTable.Load(await cmd.ExecuteReaderAsync());
                return dataTable;
            }
        }
    }

    public Task<int> UpdateTable(string query, List<DbParameter>? parameters = null) => throw new NotImplementedException("Query cannot update database");
}
