using Microsoft.AspNetCore.Mvc;
using Security.API.Controllers.BaseController;
using Security.DataServices.interfaces;
using Security.Shared.Dtos;
using Security.Shared;
using Security.Shared.Requests;
using System.ComponentModel.DataAnnotations;
namespace Security.API.Controllers
{

    /// <summary>
    /// Provides endpoints for managing system users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Tags("User Management")]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="request">The user creation request containing user details.</param>
        /// <returns>Returns the created user information.</returns>
        /// <response code="200">User created successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DataResponse<ReturnUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserRequest request)
        {
            if (request.IsModelValid())
            {
                var response = await _userService.Create(request);
                return Success(response);
            }

            return InvalidInputParameters(request.ErrorMessages());
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user.</param>
        /// <returns>Returns the user data if found.</returns>
        /// <response code="200">User retrieved successfully.</response>
        /// <response code="400">Invalid input parameter (empty GUID).</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DataResponse<ReturnUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return InvalidInputParameters(new List<ValidationResult> { new ("Invalid user ID. ID cannot be empty GUID.") });

            var userResponse = await _userService.GetById(id);
            return Success(userResponse);
        }

        /// <summary>
        /// Updates the data of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="request">The updated user data.</param>
        /// <returns>Returns the updated user information.</returns>
        /// <response code="200">User updated successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DataResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserRequest request)
        {
            if (id == Guid.Empty)
                request.AddErrorMessage(new ValidationResult("Invalid user ID. ID cannot be empty GUID."));

            if (request.IsModelValid())
            {
                var response = await _userService.Update(id, request);
                return Success(response);
            }

            return InvalidInputParameters(request.ErrorMessages());
        }

        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Returns true if deletion was successful.</returns>
        /// <response code="200">User deleted successfully.</response>
        /// <response code="404">Invalid input parameters.</response>
        /// <response code="404">User not found.</response>

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(DataResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return InvalidInputParameters(new List<ValidationResult> { new("Invalid user ID. ID cannot be empty GUID.") });

            var userResponse = await _userService.Delete(id);
            return Success(userResponse);
        }

        /// <summary>
        /// Validates the password for the specified user.
        /// </summary>
        /// <param name="id">The ID of the user whose password should be validated.</param>
        /// <param name="request">The password validation request.</param>
        /// <returns>Returns true if the password is valid, otherwise false.</returns>
        /// <response code="200">Password validated successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        /// <response code="404">User not found.</response>
        [HttpPost("{id:guid}/validate-password")]
        [ProducesResponseType(typeof(DataResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidatePassword(Guid id, [FromBody] ValidatePasswordRequest request)
        {
            if (id == Guid.Empty)
                request.AddErrorMessage(new ValidationResult("Invalid user ID. ID cannot be empty GUID."));

            if (request.IsModelValid())
            {
                var userResponse = await _userService.ValidatePassword(id, request);
                return Success(userResponse);
            }
            return InvalidInputParameters(request.ErrorMessages());
        }
    }
}
