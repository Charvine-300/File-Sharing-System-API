
using FinalYearProject.Data.Utilities;

namespace FinalYearProject.Services.UserMgmt;


public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid[] Attributes { get; set; }
}

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    List<string> Attributes
);

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}

public class UpdateUserAttributesRequest
{
    public Guid[] Attributes { get; set; } = [];
}

public record AllUsersResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsActive,
    List<string> Attributes
);

public class UserParameters : RequestParameters
{
    public bool? IsActive { get; set; }
}

public class UserStatusRequest
{
    public bool IsActive { get; set; }
}