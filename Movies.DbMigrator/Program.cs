using DbUp;
using Microsoft.Extensions.Configuration;

class Program
{
    static int Main(string[] args)
    {
        // 1. Settings
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ No se encontró la cadena de conexión.");
            Console.ResetColor();
            return -1;
        }

        // 2. Validate connection
        EnsureDatabase.For.SqlDatabase(connectionString);

        // 3. Search SQL scripts
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(
                Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Movies.Infrastructure\Database\Versioning"))
            .LogToConsole()
            .JournalToSqlTable("dbo", "DatabaseVersion")
            .Build();

        // 4. Run migrations
        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return -1;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Migraciones aplicadas correctamente.");
        Console.ResetColor();
        return 0;
    }
}
