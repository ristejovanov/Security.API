using System.ComponentModel.DataAnnotations;

namespace Security.API.Requests
{
    /// <summary>
    /// Base class for all request models that require custom validation logic.
    /// Implements <see cref="IValidatableObject"/> to allow model-level validation.
    /// </summary>
    public abstract class BaseRequestModel : IValidatableObject
    {
        private readonly List<ValidationResult> _errors = new List<ValidationResult>();

        /// <summary>
        /// Abstract method that needs to be overridden in the models to validate the model
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);

        /// <summary>
        /// Adding new error message
        /// </summary>
        /// <param name="error"></param>
        public void AddErrorMessage(ValidationResult error) => _errors.Add(error);

        /// <summary>
        /// Getting model error messages
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValidationResult> ErrorMessages() => _errors;

        /// <summary>
        /// Checking if model is valid
        /// </summary>
        /// <returns></returns>
        public bool IsModelValid() => !_errors.Any();


        /// <summary>
        /// Validate string is required
        /// </summary>
        protected void ValidateRequired(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                AddErrorMessage(new ValidationResult($"{fieldName} is required."));
        }

        /// <summary>
        /// Validate string length
        /// </summary>
        protected void ValidateStringLength(string? value, string fieldName, int minLength, int maxLength)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length < minLength || value.Length > maxLength)
                    AddErrorMessage(new ValidationResult($"{fieldName} must be between {minLength} and {maxLength} characters."));
            }
        }
    }
}
