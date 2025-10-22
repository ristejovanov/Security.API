using System.CommandLine;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Security.Data.EF.Infrastructure;

namespace Security.DbMigrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string connectionString = string.Empty;
                string userName = string.Empty;
                string password = string.Empty;
                string dataSource = string.Empty;
                string catalog = string.Empty;
                string datasourceDbConnectionString = string.Empty;

                var rootCommand = new RootCommand("Security Database Migrator");
                rootCommand.AddOption(new Option<string>(new[] { "-u", "--user" }, "Database username"));
                rootCommand.AddOption(new Option<string>(new[] { "-p", "--password" }, "Database password"));
                rootCommand.AddOption(new Option<string>(new[] { "-ds", "--datasource" }, "SQL Server name or address"));
                rootCommand.AddOption(new Option<string>(new[] { "-c", "--catalog" }, "Database name"));
                rootCommand.AddOption(new Option<string>(new[] { "-cs", "--connstring" }, "Full connection string"));
                await rootCommand.InvokeAsync(args);

                // Load configuration
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                // ✅ 1️⃣ If arguments are provided, parse and build connection
                if (args.Length > 0)
                {
                    foreach (var arg in args)
                    {
                        var argPair = arg.Split(':', 2);
                        if (argPair.Length < 2) continue;
                        var key = argPair[0].ToLower();
                        var value = argPair[1];

                        switch (key)
                        {
                            case "--connstring":
                            case "-cs":
                                connectionString = value;
                                break;
                            case "--user":
                            case "-u":
                                userName = value;
                                break;
                            case "--password":
                            case "-p":
                                password = value;
                                break;
                            case "--datasource":
                            case "-ds":
                                dataSource = value;
                                break;
                            case "--catalog":
                            case "-c":
                                catalog = value;
                                break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(connectionString))
                    {
                        datasourceDbConnectionString = connectionString + ";TrustServerCertificate=True;";
                        Console.WriteLine($"Using provided connection string: '{MaskPassword(datasourceDbConnectionString)}'");
                    }
                    else if (!string.IsNullOrWhiteSpace(dataSource) && !string.IsNullOrWhiteSpace(catalog))
                    {
                        var builder = new SqlConnectionStringBuilder
                        {
                            DataSource = dataSource,
                            InitialCatalog = catalog,
                            MultipleActiveResultSets = true,
                            TrustServerCertificate = true
                        };

                        if (string.IsNullOrWhiteSpace(userName))
                        {
                            builder.IntegratedSecurity = true;
                            builder.Encrypt = false;
                        }
                        else
                        {
                            builder.UserID = userName;
                            builder.Password = password;
                        }

                        datasourceDbConnectionString = builder.ConnectionString;
                        Console.WriteLine($"Using connection built from arguments: '{MaskPassword(datasourceDbConnectionString)}'");
                    }
                }
                // ✅ 2️⃣ If no args, try appsettings.json
                else if (!string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
                {
                    datasourceDbConnectionString = configuration.GetConnectionString("DefaultConnection");
                    Console.WriteLine("Using connection from appsettings.json:");
                    Console.WriteLine($"'{MaskPassword(datasourceDbConnectionString)}'");
                }
                // ✅ 3️⃣ Fallback to interactive mode
                else
                {
                    Console.WriteLine("No parameters or default connection found. Enter database details manually.\n");

                    Console.Write("Server name: ");
                    var serverName = Console.ReadLine()?.Trim();

                    Console.Write("Database name: ");
                    var database = Console.ReadLine()?.Trim();

                    Console.Write("Windows authentication [y/n]: ");
                    var winAuth = Console.ReadKey();
                    Console.WriteLine();

                    if (char.ToUpper(winAuth.KeyChar) == 'Y')
                    {
                        datasourceDbConnectionString =
                            $"Server={serverName};Database={database};Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
                    }
                    else
                    {
                        Console.Write("User name: ");
                        var user = Console.ReadLine();

                        Console.Write("Password: ");
                        var pass = ReadHiddenPassword();

                        datasourceDbConnectionString =
                            $"Server={serverName};Database={database};User Id={user};Password={pass};TrustServerCertificate=True;";
                    }

                    Console.WriteLine($"Using connection: '{MaskPassword(datasourceDbConnectionString)}'");
                }

                // ✅ 4️⃣ Run migrations
                Console.WriteLine("\nChecking database migrations...");
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(datasourceDbConnectionString,
                    options => options.CommandTimeout(int.MaxValue));

                using var dbContext = new AppDbContext(optionsBuilder.Options);

                var appliedMigrations = dbContext.Database.GetAppliedMigrations().ToList();
                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

                Console.WriteLine("\nApplied migrations:");
                if (appliedMigrations.Any())
                    appliedMigrations.ForEach(m => Console.WriteLine($"  ✔ {m}"));
                else
                    Console.WriteLine("  None");

                Console.WriteLine("\nPending migrations:");
                if (pendingMigrations.Any())
                    pendingMigrations.ForEach(m => Console.WriteLine($"  ➜ {m}"));
                else
                    Console.WriteLine("  None");

                if (!pendingMigrations.Any())
                {
                    Console.WriteLine("\n✅ Database is already up to date.");
                    return;
                }

                Console.WriteLine("\nPress 'Y' to apply pending migrations, or any other key to cancel.");
                var instruction = Console.ReadKey();
                Console.WriteLine();

                if (char.ToUpper(instruction.KeyChar) != 'Y')
                {
                    Console.WriteLine("Operation canceled by user.");
                    return;
                }

                Console.WriteLine("\nApplying migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("\n✅ Migration completed successfully!");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\n❌ Database connection failed:\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Unexpected error:\n{ex.Message}");
            }
        }

        private static string MaskPassword(string conn)
        {
            return Regex.Replace(conn, @"(?<=Password=)(.*?)(?=;)", "*****", RegexOptions.IgnoreCase);
        }

        private static string ReadHiddenPassword()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;
                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }
    }
}
