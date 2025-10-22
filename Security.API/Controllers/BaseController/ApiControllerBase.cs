using Microsoft.AspNetCore.Mvc;
using Security.Shared.Enums;
using Security.Shared;
using System.ComponentModel.DataAnnotations;

namespace Security.API.Controllers.BaseController
{
    /// <summary>
    /// </summary>
    public class ApiControllerBase : ControllerBase
    {
        public ApiControllerBase() : base()
        {
        }

        /// <summary>
        /// Retrieves the current Correlation ID from the HttpContext.
        /// </summary>
        protected string GetCorrelationId()
            => HttpContext?.Items.TryGetValue("CorrelationId", out var cid) == true
                ? cid?.ToString() ?? string.Empty
                : string.Empty;


        /// <summary>
        /// Returning NotFound(NotFoundResponse) with specified message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>

        protected IActionResult NotFoundResponse(string message = "Resource not found.")
        {
            var response = new DataResponse<object>
            {
                ResponseCode = EDataResponseCode.NotFound,
                ErrorMessage = message,
                CorrelationId = GetCorrelationId()
            };

            return NotFound(response);
        }

        /// <summary>
        /// Returning Ok(Success) with specified data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IActionResult Success<T>(T data)
        {
            var response = new DataResponse<T>
            {
                ResponseCode = EDataResponseCode.Success,
                Data = data,
                CorrelationId = GetCorrelationId()
            };
            return Ok(response);
        }

        /// <summary>
        /// Returning BadRequest(InvalidInputParameter) with translated error message/messages from the request validation results   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        public IActionResult InvalidInputParameters(IEnumerable<ValidationResult> validationResults)
        {
            var errorResponse = new DataResponse<object>
            {
                ResponseCode = EDataResponseCode.InvalidInputParameter,
                ErrorMessage = PrepareErrorMessage(validationResults),
                CorrelationId = GetCorrelationId()
            };
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Preparing comma separated translated messages from the validationResults
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns></returns>
        private string PrepareErrorMessage(IEnumerable<ValidationResult> validationResults)
        {
            const char messagesSeparator = ',';
            return string.Join(messagesSeparator,
                validationResults.Select(x => x.ErrorMessage + x.MemberNames.FirstOrDefault()).ToList());
        }
    }
}
