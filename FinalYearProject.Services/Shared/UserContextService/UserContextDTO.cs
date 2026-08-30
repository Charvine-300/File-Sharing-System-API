

using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Services.Shared.UserContextService;

public record CurrentUser(UserType UserType, Guid Id, string Email, string FirstName, string LastName);

//string RoleName, string RoleId, string Username,
