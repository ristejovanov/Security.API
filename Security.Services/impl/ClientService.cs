using Security.DataServices.interfaces.Helpers;
using Security.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Security.DataServices.interfaces;
using Security.Domain;
using Security.Repositories.interfaces;
using Security.Shared;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace Security.DataServices.impl
{

    namespace Application.Services
    {
        public class ClientService : IClientService
        {
            private readonly IClientRepository _clientRepo;
            private readonly IHelper _helper;
            private readonly ILogger<ClientService> _logger;
            private readonly IMemoryCache _memoryCache;
            private readonly IMapper _mapper;
            private readonly TimeSpan _timeout = TimeSpan.FromDays(1);

            public ClientService(
                IClientRepository clientRepo,
                ILogger<ClientService> logger, IHelper helper, IMemoryCache memoryCache, IMapper mapper)
            {
                _clientRepo = clientRepo;
                _logger = logger;
                _helper = helper;
                _memoryCache = memoryCache;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<ReturnClientDto> CreateClient(string clientName)
            {
                // Check for duplicates
                if (await _clientRepo.ClientNameExists(clientName))
                    throw new ConflictException($"Client '{clientName}' already exists.");

                // Generate secure API key (hash + salt)
                var (plainKey, hash, salt) = _helper.GenerateApiKey();

                var newClient = new Client
                {
                    ClientName = clientName,
                    ApiKeyHash = hash,
                    ApiKeySalt = salt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _clientRepo.Add(newClient);
                _logger.LogInformation("New client created: {ClientName}, ID={ClientId}", newClient.ClientName, newClient.ClientId);

                return new ReturnClientDto()
                {
                    ClientId = newClient.ClientId,
                    ClientName = newClient.ClientName,
                    ApiKey = plainKey // Return only once to admin
                };
            }
            
            /// <inheritdoc />
            public async Task<bool> ActivateClient(Guid clientId)
            {
                var client = await _clientRepo.GetById(clientId);
                if (client == null)
                    throw new ConflictException($"Client with client Id '{clientId}' does not exists.");


                client.IsActive = true;
                await _clientRepo.Update(client);
                _logger.LogInformation("Client activated: {ClientName} ({ClientId})", client.ClientName, client.ClientId);
                return true;
            }

            /// <inheritdoc />
            public async Task<bool> DeactivateClient(Guid clientId)
            {
                var client = await _clientRepo.GetById(clientId);
                if (client == null )
                    throw new ConflictException($"Client with client Id '{clientId}' does not exists.");


                client.IsActive = false;
                await _clientRepo.Update(client);
                _logger.LogInformation("Client deactivated: {ClientName} ({ClientId})", client.ClientName, client.ClientId);
                return true;
            }

            /// <inheritdoc />
            public async Task<ClientDto?> Validate(string apiKey)
            {
                if (!_memoryCache.TryGetValue(apiKey, out ClientDto? value))
                {
                    var clients = await _clientRepo.GetAll();
                    var client = clients.FirstOrDefault(c => _helper.Verify(apiKey, c.ApiKeySalt, c.ApiKeyHash));

                    if (client == null)
                        return null;

                    var response = _mapper.Map<ClientDto>(client);
                    _memoryCache.Set(apiKey, response,_timeout);
                    return response;
                }

                var dbResponseClient =await _clientRepo.GetById(value!.ClientId);
                if (dbResponseClient == null)
                {
                    _memoryCache.Remove(apiKey);
                    return null;
                }

                if (!_helper.Verify(apiKey, dbResponseClient.ApiKeySalt, dbResponseClient.ApiKeyHash))
                    return null;

                return _mapper.Map<ClientDto>(dbResponseClient);
            }
        }
    }

}
