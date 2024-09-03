using System.Reflection;
using DbUp;
using DbUp.Engine;
namespace DatabaseMigration
{
    public class Migration
    {
        public static DatabaseUpgradeResult CheckMigration(string connectionString)
        {
            var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            return upgrader.PerformUpgrade();
        }
    }
}
