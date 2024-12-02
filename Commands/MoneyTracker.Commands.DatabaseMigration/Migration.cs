using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using MoneyTracker.Commands.DatabaseMigration.Models;
using Npgsql;

namespace MoneyTracker.Commands.DatabaseMigration;
[ExcludeFromCodeCoverage]
public class Migration
{
    public static DatabaseUpgradeResult CheckMigration(string connectionString, MigrationOption migrationOption)
    {
        if (migrationOption.DropAllTables)
        {
            ClearAllData(connectionString);
        }

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), CreateFilter(migrationOption))
                .WithFilter(new MyScriptFilter()) // orders the scripts
                .LogToConsole()
                .Build();

        return upgrader.PerformUpgrade();
    }

    private static void ClearAllData(string connectionString)
    {
        // postgres specific and my schame specific!! cba to generalise rn
        var getAllTableNameQuery = """
            SELECT table_name
            FROM information_schema.tables 
            WHERE table_schema = 'public';
            """;
        using (var conn = new NpgsqlConnection(connectionString))
        {
            string tableNames = "";
            using (var cmd = new NpgsqlCommand(getAllTableNameQuery, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tableNames += $", {reader.GetString(0)}";
                }
                reader.Close();
            }
            if (tableNames != "")
            {
                using (var cmd = new NpgsqlCommand($"DROP TABLE {tableNames.Substring(1)}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }

    private static Func<string, bool> CreateFilter(MigrationOption migrationOption)
    {
        List<Func<string, bool>> funcLisAnd = [];
        List<Func<string, bool>> funcLisOr = [];
        funcLisAnd.Add(x => x.EndsWith(".sql"));

        funcLisOr.Add(x => x.Contains(".Migrations."));
        funcLisOr.Add(x => migrationOption.IncludeSeedData == x.EndsWith(".Seed.sql"));

        return x => funcLisAnd.All(func => func(x)) && funcLisOr.Any(func => func(x));
    }
}
