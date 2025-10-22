using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Security.API.Controllers.BaseController;
using Security.DataServices.interfaces;
using Security.Shared;
using Security.Shared.Dtos;
using Security.Shared.Requests;
namespace Security.API.Controllers
{

    /// <summary>
    /// Provides operations for managing system clients.
    /// </summary>
    
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Tags("Client Management")]
    public class ClientController : ApiControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Creates a new client in the system.
        /// </summary>
        /// <param name="request">The client creation request.</param>
        /// <returns>A success or error response.</returns>
        /// <response code="200">Client created successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(typeof(DataResponse<ReturnClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ClientRequest request)
        {
            if (request.IsModelValid())
            {
                var response = await _clientService.CreateClient(request.ClientName);
                return Success(response);
            }

            return InvalidInputParameters(request.ErrorMessages());
        }


        /// <summary>
        /// Activates a client by ID.
        /// </summary>
        /// <param name="id">The ID of the client to activate.</param>
        /// <returns>Activation result.</returns>
        /// <response code="200">Client activated successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        /// <response code="404">Client not found.</response>
        [HttpPatch("{id:guid}/activate")]
        [ProducesResponseType(typeof(DataResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Activate(Guid id)
        {
            if (Guid.Empty != id)
            {
                var response = await _clientService.ActivateClient(id);
                return Success(response);
            }

            return InvalidInputParameters(new List<ValidationResult>{new ValidationResult("Invalid input param")});
        }

        /// <summary>
        /// Deactivates a client by ID.
        /// </summary>
        /// <param name="id">The ID of the client to deactivate.</param>
        /// <returns>Deactivation result.</returns>
        /// <response code="200">Client deactivated successfully.</response>
        /// <response code="400">Invalid input parameters.</response>
        /// <response code="404">Client not found.</response>
        [HttpPatch("{id:guid}/deactivate")]
        [ProducesResponseType(typeof(DataResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            if (Guid.Empty != id)
            {
                var response = await _clientService.DeactivateClient(id);
                return Success(response);
            }

            return InvalidInputParameters(new List<ValidationResult> { new ValidationResult("Invalid input param") });
        }
    }
}
