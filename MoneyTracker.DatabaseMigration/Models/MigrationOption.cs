namespace MoneyTracker.DatabaseMigration.Models;

public class MigrationOption(bool includeSeedData = false)
{
    public bool IncludeSeedData { get; private set; } = includeSeedData;
}
