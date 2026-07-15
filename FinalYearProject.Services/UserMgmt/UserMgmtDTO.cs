
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.Shared.Validators;
using Microsoft.AspNetCore.Http;

namespace FinalYearProject.Services.UserMgmt;


public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid[] Attributes { get; set; }

    [AllowedImageExtensions]
    public IFormFile? ProfilePhoto { get; set; }
}

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [AllowedImageExtensions]
    public IFormFile? ProfilePhoto { get; set; }
}

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string ProfilePhoto,
    List<string> Attributes
);


public class UpdateUserAttributesRequest
{
    public Guid[] Attributes { get; set; } = [];
}

public record AllUsersResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string ProfilePhoto,
    bool IsActive
);

public class UserParameters : RequestParameters
{
    public bool? IsActive { get; set; }
}

public class UserStatusRequest
{
    public bool IsActive { get; set; }
}