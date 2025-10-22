using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Security.DataServices.interfaces.Helpers;
using Security.Domain;
using Security.Repositories.interfaces;

namespace Security.DataServices.Service
{
    public class AdminClientService : IHostedService
    {
        private readonly ILogger<AdminClientService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHelper _helper;
        private readonly string _keyFileDir = Path.Combine(AppContext.BaseDirectory, "Security");
        private readonly string _keyFileName = "admin-api-key.txt";

        private static readonly Guid AdminClientId = new("11111111-1111-1111-1111-111111111111");

        public AdminClientService(
            ILogger<AdminClientService> logger,
            IHelper helper, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _helper = helper;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //can not provide direct DI from IClientRepository because it is scope service 
                using var scope = _scopeFactory.CreateScope();
                var clientRepo = scope.ServiceProvider.GetRequiredService<IClientRepository>();

                _logger.LogInformation("🔍 Checking for default admin client...");

                var existing = await clientRepo.GetById(AdminClientId);
                if (existing != null)
                {
                    _logger.LogInformation("✅ Admin client already exists. No action required.");
                    return;
                }

                _logger.LogInformation("⚙️ Admin client not found — creating a new one...");

                // Generate secure API key
                var (plainKey, hash, salt) = _helper.GenerateApiKey();

                var admin = new Client
                {
                    ClientId = AdminClientId,
                    ClientName = "AdminClient",
                    ApiKeyHash = hash,
                    ApiKeySalt = salt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await clientRepo.Add(admin);
                _logger.LogInformation("✅ Admin client created successfully in database.");

                // Save the plain API key to a file
                WriteApiKeyToFile(plainKey);

                _logger.LogWarning("⚠️ ADMIN API KEY generated and saved to file '{FilePath}'.", Path.Combine(_keyFileDir, _keyFileName));
                _logger.LogInformation("🛠️ Please store it securely — it will not be shown again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to create admin client.");
                _logger.LogError(ex, "❌ Restart the application and try again.");
            }
        }


        // -ToDo !!! for professional store the key in Azure Key Vault or a similar secure store
        private void WriteApiKeyToFile(string plainKey)
        {
            var filePath = Path.Combine(_keyFileDir, _keyFileName);

            if (!Directory.Exists(_keyFileDir))
                Directory.CreateDirectory(_keyFileDir);

            if (File.Exists(filePath))
                File.Delete(filePath);

            var content = new StringBuilder()
                .AppendLine("=====================================")
                .AppendLine(" ADMIN CLIENT API KEY (Store Securely)")
                .AppendLine("=====================================")
                .AppendLine($"Generated At: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC")
                .AppendLine($"Client ID: {AdminClientId}")
                .AppendLine($"API Key: {plainKey}")
                .AppendLine("=====================================")
                .ToString();

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 AdminClientInitializerService stopped.");
            return Task.CompletedTask;
        }
    }
}
