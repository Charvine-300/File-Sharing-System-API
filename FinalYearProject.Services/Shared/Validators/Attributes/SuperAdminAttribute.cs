using FinalYearProject.Data.Domain.Entities.Shared;
using FinalYearProject.Data.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FinalYearProject.Services.Shared.Validators.Attributes;

public class SuperAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!(user.Identity?.IsAuthenticated ?? false))
        {
            context.Result = new ObjectResult(
                new ApiResponse(false,
                    (int)HttpStatusCode.Unauthorized,
                    "Authentication required."))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            return;
        }

        var userType = user.FindFirst("userType")?.Value;

        if (!Enum.TryParse<UserType>(userType, true, out var parsedType) ||
            parsedType != UserType.SuperAdmin)
        {
            context.Result = new ObjectResult(
                new ApiResponse(false,
                    (int)HttpStatusCode.Forbidden,
                    "Only Super Administrators can perform this action."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}