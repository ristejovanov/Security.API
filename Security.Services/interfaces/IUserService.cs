using Security.Shared.Dtos;
using Security.Shared.Requests;


namespace Security.DataServices.interfaces
{
    /// <summary>
    /// Defines operations for managing users within the security system.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user in the system based on the provided user details.
        /// </summary>
        /// <param name="userDto">The user creation request containing user information such as username, email, and password.</param>
        /// <returns>
        /// A <see cref="CreateUserDto"/> containing information about the newly created user.
        /// </returns>
        Task<CreateUserDto> Create(UserRequest userDto);

        /// <summary>
        /// Retrieves a user by their unique identifier (GUID).
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>
        /// A <see cref="ReturnUserDto"/> containing user details if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ReturnUserDto?> GetById(Guid id);

        /// <summary>
        /// Updates the data of an existing user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="userRequest">The updated user details.</param>
        /// <returns>
        /// <c>true</c> if the user was successfully updated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Update(Guid id, UserRequest userRequest);

        /// <summary>
        /// Deletes a user from the system by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>
        /// <c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Delete(Guid id);

        /// <summary>
        /// Validates whether the provided password matches the stored password for a given user.
        /// </summary>
        /// <param name="id">The unique identifier of the user whose password will be validated.</param>
        /// <param name="password">The password validation request containing the plain-text password to verify.</param>
        /// <returns>
        /// <c>true</c> if the provided password is valid; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ValidatePassword(Guid id, ValidatePasswordRequest password);
    }
}
