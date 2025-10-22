using Security.Domain;

namespace Security.Repositories.interfaces
{
    /// <summary>
    /// Defines data access operations for managing <see cref="User"/> entities in the persistence layer.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a new <see cref="User"/> entity to the data store.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task Add(User user);

        /// <summary>
        /// Updates an existing <see cref="User"/> entity in the data store.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>
        /// <c>true</c> if the user was successfully updated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Update(User user);

        /// <summary>
        /// Checks whether a user with the specified email address already exists in the system.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>
        /// <c>true</c> if a user with the given email exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> EmailExists(string email);

        /// <summary>
        /// Checks whether a user with the specified username already exists in the system.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>
        /// <c>true</c> if a user with the given username exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UserNameExists(string userName);

        /// <summary>
        /// Retrieves a <see cref="User"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user.</param>
        /// <returns>
        /// The matching <see cref="User"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<User?> GetById(Guid id);

        /// <summary>
        /// Deletes a <see cref="User"/> entity from the data store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user to delete.</param>
        /// <returns>
        /// <c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Delete(Guid id);
    }
}
