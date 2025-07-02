using System.Data;
using System.Data.Common;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class PostgresDatabase : IDatabase
{
    private readonly NpgsqlDataSource _dataSource_rw;

    // TODO: have ro and rw strings
    public PostgresDatabase(string connectionString)
    {
        NpgsqlDataSourceBuilder dataSourceBuilder_ro = new NpgsqlDataSourceBuilder(connectionString);
        _dataSource_rw = dataSourceBuilder_ro.Build();
    }

    public async Task<DataTable> GetTable(string query,
        CancellationToken cancellationToken, List<DbParameter>? parameters = null)
    {
        await using (var conn = await _dataSource_rw.OpenConnectionAsync(cancellationToken))
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
                dataTable.Load(await cmd.ExecuteReaderAsync(cancellationToken));
                return dataTable;
            }
        }
    }

    public async Task<int> UpdateTable(string query, CancellationToken cancellationToken,
        List<DbParameter>? parameters = null)
    {
        await using (var conn = await _dataSource_rw.OpenConnectionAsync(cancellationToken))
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
                return await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}
