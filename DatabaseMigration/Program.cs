using System.Reflection;
using DatabaseMigration;
using DbUp;
using Microsoft.Extensions.Configuration;

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

        var result = Migration.CheckMigration(connectionString);
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
