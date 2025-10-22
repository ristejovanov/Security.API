using Security.Domain;
namespace Security.Repositories.interfaces
{
    /// <summary>
    /// Defines data access operations for managing <see cref="Client"/> entities in the persistence layer.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Retrieves all <see cref="Client"/> entities from the data store.
        /// </summary>
        /// <returns>
        /// A collection of all registered <see cref="Client"/> entities.
        /// </returns>
        Task<IEnumerable<Client>> GetAll();

        /// <summary>
        /// Retrieves a specific <see cref="Client"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the client to retrieve.</param>
        /// <returns>
        /// The matching <see cref="Client"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Client?> GetById(Guid id);

        /// <summary>
        /// Checks whether a client with the specified name already exists in the system.
        /// </summary>
        /// <param name="clientName">The name of the client to check for existence.</param>
        /// <returns>
        /// <c>true</c> if a client with the specified name exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ClientNameExists(string clientName);

        /// <summary>
        /// Adds a new <see cref="Client"/> entity to the data store.
        /// </summary>
        /// <param name="client">The client entity to add.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task Add(Client client);

        /// <summary>
        /// Updates an existing <see cref="Client"/> entity in the data store.
        /// </summary>
        /// <param name="client">The client entity containing updated information.</param>
        /// <returns>
        /// <c>true</c> if the client was successfully updated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Update(Client client);
    }
}
