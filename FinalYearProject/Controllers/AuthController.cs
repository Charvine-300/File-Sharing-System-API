using FinalYearProject.Controllers.Shared;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.AuthMgmt;
using Microsoft.AspNetCore.Mvc;


namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthController(IAuthService authService) : BaseController
{
    /// <summary>
    /// Log in to account
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    //[HasPermission("trail.read")]

    [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    [ProducesResponseType(404, Type = typeof(ApiResponse))]
    public async Task<IActionResult> Login([FromBody] LoginRequest parameters, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ServiceResponse<LoginResponse> response = await authService.LoginAsync(parameters, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Change password 
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("change-password")]
    //[HasPermission("trail.read")]

    [ProducesResponseType(200, Type = typeof(ApiResponse))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    [ProducesResponseType(404, Type = typeof(ApiResponse))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest parameters, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ServiceResponse response = await authService.ChangePasswordAsync(parameters, cancellationToken);
        return ComputeResponse(response);
    }
}
