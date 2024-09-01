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

        return Migration.CheckMigration(connectionString);
    }
}
