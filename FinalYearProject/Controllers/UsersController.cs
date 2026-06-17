using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.UserMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/users")]
//[Authorize]
public class UsersController(IUserMgmtService userService) : BaseController
{
    /// <summary>
    /// Get details of a particular user
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await userService.GetUserAsync(id, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Create a new user with the provided details
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await userService.CreateUserAsync(request, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Delete a user by their unique identifier
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await userService.DeleteUserAsync(id, cancellationToken);
        return ComputeResponse(response);
    }
}
