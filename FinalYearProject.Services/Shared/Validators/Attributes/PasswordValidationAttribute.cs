using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FinalYearProject.Services.Validators.Attributes
{

    public class PasswordValidationAttribute(int minLength = 8) : ValidationAttribute
    {
        private readonly int _minLength = minLength;

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("Password is required.");
            }

            string password = value.ToString()!;
            if (password.Length < _minLength)
                return new ValidationResult($"Password must be at least {_minLength} characters long.");

            if (!Regex.IsMatch(password, "[A-Z]"))
                return new ValidationResult("Password must contain at least one uppercase letter.");
            if (!Regex.IsMatch(password, "[a-z]"))
                return new ValidationResult("Password must contain at least one lowercase letter.");
            if (!Regex.IsMatch(password, "[0-9]"))
                return new ValidationResult("Password must contain at least one number.");
            if (!Regex.IsMatch(password, @"[\W_]"))
                return new ValidationResult("Password must contain at least one special character.");

            return ValidationResult.Success!;
        }
    }

}
