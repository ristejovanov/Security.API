using System.CommandLine;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Security.Data.EF.Infrastructure;

namespace Security.DbMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string connectionString = String.Empty;
                string userName = string.Empty;
                string password = "";
                string dataSource = string.Empty;
                string catalog = string.Empty;
                string datasourceDbConnectionString = string.Empty;
                bool hasArgs = true;
                var hidePassword = "";

                var rootCommand = new RootCommand();
                rootCommand.Name = "DatasourceDbMigrator";
                rootCommand.Description = "Datasource Db Migrator";

                var userOption = new Option<string>(new[] { "-u", "--user" }, "Enter User Name.")
                    { ArgumentHelpName = "User Name" };
                var passwordOption = new Option<string>(new[] { "-p", "--password" }, "Enter Password.")
                    { ArgumentHelpName = "Password" };
                var dataSourceOption = new Option<string>(new[] { "-ds", "--datasource" }, "Enter Datasource.")
                    { ArgumentHelpName = "Server Name" };
                var catalogOption = new Option<string>(new[] { "-c", "--catalog" }, "Enter Catalog.")
                    { ArgumentHelpName = "Database Name" };
                var connStringOption =
                    new Option<string>(new[] { "-cs", "--connstring" },
                            "Enter Connection string in format: Server=ServerName;Database=DatabaseName;user id=UserName;Password=pass;")
                        { ArgumentHelpName = "Connection String" };

                rootCommand.Add(userOption);
                rootCommand.Add(passwordOption);
                rootCommand.Add(dataSourceOption);
                rootCommand.Add(catalogOption);
                rootCommand.Add(connStringOption);

                await rootCommand.InvokeAsync(args);

                if (args.Length > 0)
                {
                    foreach (var arg in args)
                    {
                        var argKeyValuePair = arg.Split(':');
                        switch (argKeyValuePair[0].ToLower())
                        {
                            case "--connstring":
                            case "-cs":
                                connectionString = argKeyValuePair[1];
                                break;
                            case "--user":
                            case "-u":
                                userName = argKeyValuePair[1];
                                break;
                            case "--password":
                            case "-p":
                                password = argKeyValuePair[1];
                                hidePassword = password;
                                hidePassword = hidePassword.Replace(hidePassword, "*****");
                                break;
                            case "--datasource":
                            case "-ds":
                                dataSource = argKeyValuePair[1];
                                break;
                            case "--catalog":
                            case "-c":
                                catalog = argKeyValuePair[1];
                                break;
                            default:
                                hasArgs = false;
                                break;
                        }
                    }
                }

                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();


                var connStringArg = "Data Source=" + dataSource + ";Initial Catalog =" + catalog + ";User ID =" +
                                    userName +
                                    ";Password=" + hidePassword +
                                    ";Multiple Active Result Sets = True;TrustServerCertificate=True;";
                if (!string.IsNullOrEmpty(connectionString))
                {
                    datasourceDbConnectionString = connectionString + "TrustServerCertificate=True;";
                    Console.WriteLine(
                        $"Your connection string is: '{Regex.Replace(datasourceDbConnectionString, @"(?<=Password=)(.*)(?=\;)", "*****")}'");
                }
                else if (dataSource != null && catalog != null)
                {
                    var connStringBuilder = new SqlConnectionStringBuilder
                    {
                        DataSource = dataSource,
                        InitialCatalog = catalog,
                        MultipleActiveResultSets = true
                    };

                    if (userName == null)
                    {
                        connStringBuilder.IntegratedSecurity = true;
                        connStringBuilder.Encrypt = false;
                    }
                    else
                    {
                        connStringBuilder.UserID = userName;
                        connStringBuilder.Password = password;
                        connStringBuilder.TrustServerCertificate = true;
                    }

                    datasourceDbConnectionString = connStringBuilder.ConnectionString;
                    Console.WriteLine(
                        $"Your connection string is: '{Regex.Replace(datasourceDbConnectionString, @"(?<=Password=)(.*)(?=\;)", "*****")}'");
                }
                else if (!string.IsNullOrEmpty(configuration.GetConnectionString("DefaultConnection")))
                {
                    datasourceDbConnectionString = configuration.GetConnectionString("DefaultConnection");
                }
                else if (hasArgs == false)
                    return;
                else if (hasArgs && args.Length > 1)
                    Console.WriteLine($"Your connection string is: {connStringArg}");
                else
                {
                    Console.WriteLine("Enter the connection string for the database that you want to update.");
                    Console.Write("Server name = ");
                    var serverName = Console.ReadLine();

                    Console.Write("DatabaseName = ");
                    var database = Console.ReadLine();

                    Console.WriteLine("Windows authentication [y/n]");
                    var winAuth = Console.ReadKey();
                    Console.WriteLine();

                    if (char.ToUpper(winAuth.KeyChar) == 'N')
                    {
                        Console.Write("UserName = ");
                        var user = Console.ReadLine();
                        Console.Write("Password = ");
                        var pass = string.Empty;
                        ConsoleKey key1;
                        do
                        {
                            var keyInfo = Console.ReadKey(intercept: true);
                            key1 = keyInfo.Key;
                            if (key1 == ConsoleKey.Backspace && pass.Length > 0)
                            {
                                Console.Write("\b \b");
                                pass = pass[0..^1];
                            }
                            else if (!char.IsControl(keyInfo.KeyChar))
                            {
                                Console.Write("*");
                                pass += keyInfo.KeyChar;
                            }
                        } while (key1 != ConsoleKey.Enter);

                        Console.WriteLine();

                        datasourceDbConnectionString =
                            "Server=" + serverName + ";Database=" + database + ";user id=" + user +
                            ";Password=" + pass + ";TrustServerCertificate=True;";
                    }
                    else if (char.ToUpper(winAuth.KeyChar) == 'Y')
                    {
                        datasourceDbConnectionString = "Server=" + serverName + ";Database=" + database +
                                                       ";Integrated Security=True;Encrypt=False";
                    }
                    else
                        return;
                }


                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(datasourceDbConnectionString,
                    options => options.CommandTimeout(int.MaxValue));
                var dbContext = new AppDbContext(optionsBuilder.Options);

                var appliedMigrations = dbContext.Database.GetAppliedMigrations();
                Console.WriteLine("Existing database schemas:");
                if (!appliedMigrations.Any())
                    Console.WriteLine("None");
                foreach (var migration in appliedMigrations)
                {
                    Console.WriteLine(migration);
                }

                var pendingMigrations = dbContext.Database.GetPendingMigrations();

                Console.WriteLine("Pending database schemas:");
                if (!pendingMigrations.Any())
                    Console.WriteLine("None");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine(migration);
                }

                Console.WriteLine("Press 'Y' to continue update or any key to exit.");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (char.ToUpper(key.KeyChar) != 'Y')
                    return;

                if (!appliedMigrations.Any())
                    Console.WriteLine("\nCreating database...");
                else
                    Console.WriteLine("\nUpdating database...");

                dbContext.Database.Migrate();

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Failed to connect to database. \n" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application failed. \n" + ex.Message);
            }
        }
    }
}