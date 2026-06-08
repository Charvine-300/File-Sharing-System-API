using Serilog;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Services.Shared.UserContextService;

public class UserContextService(IHttpContextAccessor httpContextAccessor): IUserContextService
{
    public CurrentUser User => GetCurrentUser();

    private CurrentUser GetCurrentUser()
    {
        var user = httpContextAccessor?.HttpContext?.User;

        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return new CurrentUser(default, default, string.Empty, string.Empty, string.Empty);
        }


        var userTypeClaim = user.FindFirst("userType")?.Value;

        UserType userType = Enum.TryParse<UserType>(
            userTypeClaim,
            true,
            out var parsedUserType
        )
            ? parsedUserType
            : UserType.Regular;
        var userId = user.FindFirst("id")?.Value ?? string.Empty;
        var firstName = user.FindFirst("firstName")?.Value ?? string.Empty;
        var lastName = user.FindFirst("lastName")?.Value ?? string.Empty;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

        var role = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .FirstOrDefault() ?? string.Empty;

        // Log missing claims (for debugging)
        if (string.IsNullOrWhiteSpace(email)) Log.Error("Email claim is null.");
        if (string.IsNullOrWhiteSpace(userId)) Log.Error("UserId claim is null.");
        //if (string.IsNullOrWhiteSpace(roleId)) Log.Error("RoleId claim is null.");

        return new CurrentUser(userType, userId, email, firstName, lastName);
    }
}
