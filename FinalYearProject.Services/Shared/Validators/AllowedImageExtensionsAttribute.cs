using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace FinalYearProject.Services.Shared.Validators;

public class AllowedImageExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not IFormFile file)
            return new ValidationResult("Invalid file.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_extensions.Contains(extension))
        {
            return new ValidationResult(
                $"Only {string.Join(", ", _extensions)} files are allowed.");
        }

        return ValidationResult.Success;
    }
}
