using FinalYearProject.Data.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace FinalYearProject.Services.Validators.Attributes;

public class HasPermissionAttribute(string requiredPermission) : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClaimsPrincipal user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ObjectResult(new ApiResponse(false, (int)HttpStatusCode.Unauthorized, "Authentication required."))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        string? permissionsClaim = user.FindFirst("permissions")?.Value;
        if (string.IsNullOrWhiteSpace(permissionsClaim))
        {
            context.Result = new ObjectResult(new ApiResponse(false, (int)HttpStatusCode.Forbidden, "Missing permissions claim."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        string[] permissions = permissionsClaim.Split(',').Select(p => p.Trim().ToLower()).ToArray();
        string[] requiredPermissions = requiredPermission.Split(',').Select(p => p.Trim().ToLower()).ToArray();
        if (!requiredPermissions.Any(rp => permissions.Contains(rp)))
        {
            context.Result = new ObjectResult(new ApiResponse(false, (int)HttpStatusCode.Forbidden, "You don't have the required permissions to perform this action. Please contact your admin."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
