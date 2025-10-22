using System.ComponentModel.DataAnnotations;
using Security.API.Requests;

namespace Security.Shared.Requests
{
    public class ValidatePasswordRequest : BaseRequestModel
    {
        public string Password { get; set; } = string.Empty;
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidateRequired(Password, nameof(Password));

            return ErrorMessages();
        }
    }
}
