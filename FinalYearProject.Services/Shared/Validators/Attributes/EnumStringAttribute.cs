namespace FinalYearProject.Services.Validators.Attributes;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EnumStringAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumStringAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum");

        _enumType = enumType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult($"{validationContext.DisplayName} is required.");

        if (value is not string stringValue)
            return new ValidationResult($"{validationContext.DisplayName} must be a string.");

        // 🔒 CASE-SENSITIVE check
        var enumNames = Enum.GetNames(_enumType);

        if (enumNames.Contains(stringValue))
            return ValidationResult.Success!;

        return new ValidationResult(
            $"{validationContext.DisplayName} must be one of: {string.Join(", ", enumNames)}"
        );
    }
}

