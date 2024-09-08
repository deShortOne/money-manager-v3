using System.Reflection;
using DbUp;
using DbUp.Engine;
using MoneyTracker.DatabaseMigration.Models;

namespace MoneyTracker.DatabaseMigration;

public class Migration
{
    public static DatabaseUpgradeResult CheckMigration(string connectionString, MigrationOption migrationOption)
    {
        var upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), CreateFilter(migrationOption))
            .WithFilter(new MyScriptFilter()) // orders the scripts
            .LogToConsole()
            .Build();

        return upgrader.PerformUpgrade();
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
