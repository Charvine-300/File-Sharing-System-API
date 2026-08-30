using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FinalYearProject.Data.Utilities;

public class StartsWithAttribute : ValidationAttribute
{
    private readonly string _prefix;

    public StartsWithAttribute(string prefix)
    {
        _prefix = prefix;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return new ValidationResult($"{validationContext.DisplayName} is required.");
        }

        var stringValue = value.ToString();

        if (!stringValue.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} must start with '{_prefix}'."
            );
        }

        return ValidationResult.Success;
    }

    public class ValidImgTypeAndSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;

        private readonly string[] _permittedExtensions =
        {
        ".jpg", ".jpeg", ".png", ".svg"
    };

        public ValidImgTypeAndSizeAttribute(int maxFileSizeInMB = 5)
        {
            _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as IEnumerable<IFormFile>;

            if (files == null || !files.Any())
            {
                return new ValidationResult("At least one image is required.");
            }

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    return new ValidationResult("One of the uploaded files is empty.");
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!_permittedExtensions.Contains(extension))
                {
                    return new ValidationResult(
                        $"{file.FileName} has invalid file type. Allowed types: {string.Join(", ", _permittedExtensions)}"
                    );
                }

                if (file.Length > _maxFileSizeInBytes)
                {
                    return new ValidationResult(
                        $"{file.FileName} exceeds {_maxFileSizeInBytes / (1024 * 1024)}MB."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }

    public class ValidImgTypeAttribute : ValidationAttribute
    {
        private static readonly string[] AllowedTypes = new[] { "avatar", "cover" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var imgType = value as string;

            if (string.IsNullOrWhiteSpace(imgType))
            {
                return new ValidationResult("ImgType is required.");
            }

            if (!AllowedTypes.Contains(imgType.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(
                    $"'{imgType}' is not a valid image type. Allowed values are: {string.Join(", ", AllowedTypes)}");
            }

            return ValidationResult.Success;
        }
    }

    public class MatricNoAttribute : ValidationAttribute
    {
        private const string Pattern = @"^\d{9}$";

        public MatricNoAttribute()
        {
            ErrorMessage = "MatricNo must be exactly 9 digits.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("MatricNo is required.");
            }

            if (value is not string matricNo)
            {
                return new ValidationResult("Invalid MatricNo format.");
            }

            if (!Regex.IsMatch(matricNo, Pattern))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    public class ValidAttributeNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(
                    "Attribute name is required");
            }

            string attributeName = value.ToString()!;

            // Only allow letters and numbers.
            // No spaces, symbols, underscores, etc.
            bool isValid = Regex.IsMatch(
                attributeName,
                @"^[A-Za-z][A-Za-z0-9]*$"
            );

            if (!isValid)
            {
                return new ValidationResult(
                    "Attribute name must be camel-cased and contain only letters and numbers. Example: SoftwareEngineer, DataScientist.");
            }

            return ValidationResult.Success;
        }
    }
}