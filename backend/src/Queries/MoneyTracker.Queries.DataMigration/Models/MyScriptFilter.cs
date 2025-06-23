using System.Diagnostics.CodeAnalysis;
using DbUp.Engine;
using DbUp.Support;

namespace MoneyTracker.Queries.DatabaseMigration.Models;
[ExcludeFromCodeCoverage]
internal class MyScriptFilter : IScriptFilter
{
    public IEnumerable<SqlScript> Filter(
        IEnumerable<SqlScript> sorted,
        HashSet<string> executedScriptNames,
        ScriptNameComparer comparer)
    {
        return sorted
            .Where(s => s.SqlScriptOptions.ScriptType == ScriptType.RunAlways
                        || !executedScriptNames.Contains(s.Name, comparer))
            .OrderBy(script => script.Name);
    }
}
