using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Security.API.Requests;

namespace Security.Shared.Requests
{
    public class UserRequest : BaseRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Language { get; set; } = "en";
        public string Culture { get; set; } = "en-US";
        public string Password { get; set; } = string.Empty;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validation required prop
            ValidateRequired(UserName, nameof(UserName));
            ValidateRequired(FullName, nameof(FullName));
            ValidateRequired(Email, nameof(Email));
            ValidateRequired(MobileNumber, nameof(MobileNumber));
            ValidateRequired(Culture, nameof(Culture));
            ValidateRequired(Password, nameof(Password));

            // Validate lengths
            ValidateStringLength(UserName, nameof(UserName), 3, 50);
            ValidateStringLength(FullName, nameof(FullName), 1, 100);
            ValidateStringLength(Email, nameof(Email), 5, 100);
            ValidateStringLength(MobileNumber, nameof(MobileNumber), 0, 20);
            ValidateStringLength(Language, nameof(Language), 2, 10);
            ValidateStringLength(Culture, nameof(Culture), 2, 10);


            if (!string.IsNullOrWhiteSpace(MobileNumber))
            {
                var attr = new PhoneAttribute();
                if (!attr.IsValid(MobileNumber))
                    AddErrorMessage(new ValidationResult("Invalid MobileNumber format."));
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                var attr = new EmailAddressAttribute();
                if (!attr.IsValid(Email))
                    AddErrorMessage(new ValidationResult("Invalid Email format."));
            }

            if (string.IsNullOrWhiteSpace(Password))
                AddErrorMessage(new ValidationResult("Password shouldn't contain white spaces."));

            var hasUpper = Regex.IsMatch(Password, "[A-Z]");
            var hasLower = Regex.IsMatch(Password, "[a-z]");
            var hasDigit = Regex.IsMatch(Password, "[0-9]");
            var hasSpecial = Regex.IsMatch(Password, "[^a-zA-Z0-9]");
            var isLongEnough = Password.Length >= 8;

            if (!(hasUpper && hasLower && hasDigit && hasSpecial && isLongEnough))
            {
                AddErrorMessage(new ValidationResult(
                    "Password must contain at least 8 characters, one uppercase, one lowercase, one digit, and one special character."));
            }

            return ErrorMessages();
        }
    }
}
