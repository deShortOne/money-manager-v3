using Npgsql;

namespace MoneyTracker.API.Database
{
    public class Helper
    {
        static IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<SecretKey>()
                .Build();

        static readonly NpgsqlDataSourceBuilder dataSourceBuilder_ro = new NpgsqlDataSourceBuilder(config["Database:Paelagus_RO"]);
        static readonly NpgsqlDataSource dataSource_ro = dataSourceBuilder_ro.Build();

        static readonly NpgsqlDataSourceBuilder dataSourceBuilder_rw = new NpgsqlDataSourceBuilder(config["Database:Paelagus_RO"]);
        static readonly NpgsqlDataSource dataSource_rw = dataSourceBuilder_rw.Build();

        public static async Task<NpgsqlDataReader> GetTable(string query, List<NpgsqlParameter> parameters = null)
        {
            var conn = await dataSource_ro.OpenConnectionAsync();

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

        public static async Task<int> UpdateTable(string query, List<NpgsqlParameter> parameters = null)
        {
            var conn = await dataSource_rw.OpenConnectionAsync();

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
