using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MoneyTracker.Data.Global;
using Npgsql;

namespace MoneyTracker.Data.Postgres
{
    public class PostgresDatabase : IDatabase
    {
        private readonly NpgsqlDataSource _dataSource_rw;

        // TODO: have ro and rw strings
        public PostgresDatabase(string connectionString)
        {
            NpgsqlDataSourceBuilder dataSourceBuilder_ro = new NpgsqlDataSourceBuilder(connectionString);
            _dataSource_rw = dataSourceBuilder_ro.Build();
        }

        public async Task<DbDataReader> GetTable(string query, List<DbParameter> parameters = null)
        {
            var conn = await _dataSource_rw.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                return await cmd.ExecuteReaderAsync();
            }
        }

        public async Task<int> UpdateTable(string query, List<DbParameter> parameters = null)
        {
            var conn = await _dataSource_rw.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
