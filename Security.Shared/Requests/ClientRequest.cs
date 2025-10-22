using System.ComponentModel.DataAnnotations;
using Security.API.Requests;

namespace Security.Shared.Requests
{
    public class ClientRequest : BaseRequestModel
    {
        public string ClientName { get; set; } = string.Empty;
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidateRequired(ClientName, nameof(ClientName));
            ValidateStringLength(ClientName, nameof(ClientName), 3, 50);

            return ErrorMessages();
        }
    }
}
