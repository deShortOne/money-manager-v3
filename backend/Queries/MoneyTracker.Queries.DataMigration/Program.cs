
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using MoneyTracker.Queries.DatabaseMigration.Models;

namespace MoneyTracker.Queries.DatabaseMigration;
[ExcludeFromCodeCoverage]
public class Program
{
    public static int Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<SecretKey>()
            .Build();

        var connectionString =
            args.FirstOrDefault()
            ?? config["Database:Paelagus_RO"]
            ?? "ERROR CONNECTION STRING NOT FOUND";

        var result = Migration.CheckMigration(connectionString, new MigrationOption(true, true));
        if (result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
#endif
            return -1;
        }
    }
}
