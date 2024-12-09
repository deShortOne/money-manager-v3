
using System.Diagnostics.CodeAnalysis;

namespace MoneyTracker.Commands.DatabaseMigration.Models;
[ExcludeFromCodeCoverage]
public class MigrationOption(bool includeSeedData = false, bool dropAllTables = false)
{
    public bool IncludeSeedData { get; } = includeSeedData;
    public bool DropAllTables { get; } = dropAllTables;
}
