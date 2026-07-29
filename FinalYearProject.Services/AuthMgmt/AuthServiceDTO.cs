using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Services.Validators.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Services.AuthMgmt;

public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiryTimeStamp, string FirstName, string LastName, string UserType, Guid UserId, string ProfilePhotoUrl);

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

public class ChangePasswordRequest
{

    [Required]
    public string OldPassword { get; set; }

    [Required]
    [PasswordValidation]
    public string NewPassword { get; set; }

    [Required]
    [PasswordValidation]
    [Compare("NewPassword")]
    public string? ConfirmPassword { get; set; }
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;

    [PasswordValidation]
    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}

public class VerifyResetOtpRequest
{
    public string Email { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}