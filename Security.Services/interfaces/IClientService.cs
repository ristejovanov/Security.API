using Security.Shared.Dtos;
using System;
using System.Threading.Tasks;

namespace Security.DataServices.interfaces
{
    /// <summary>
    /// Defines operations for managing and validating system clients.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Creates a new client within the security system.
        /// </summary>
        /// <param name="clientName">The name of the client to create.</param>
        /// <returns>
        /// A <see cref="ReturnClientDto"/> containing information about the newly created client.
        /// </returns>
        Task<ReturnClientDto> CreateClient(string clientName);

        /// <summary>
        /// Activates a client, enabling it to authenticate and use the system.
        /// </summary>
        /// <param name="clientId">The unique identifier (GUID) of the client to activate.</param>
        /// <returns>
        /// <c>true</c> if the client was successfully activated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ActivateClient(Guid clientId);

        /// <summary>
        /// Deactivates a client, preventing it from authenticating or using the system.
        /// </summary>
        /// <param name="clientId">The unique identifier (GUID) of the client to deactivate.</param>
        /// <returns>
        /// <c>true</c> if the client was successfully deactivated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeactivateClient(Guid clientId);

        /// <summary>
        /// Validates an API key and retrieves information about the corresponding client.
        /// </summary>
        /// <param name="apiKey">The API key used to authenticate the client.</param>
        /// <returns>
        /// A <see cref="ClientDto"/> representing the validated client if the API key is valid; otherwise, <c>null</c>.
        /// </returns>
        Task<ClientDto?> Validate(string apiKey);
    }
}