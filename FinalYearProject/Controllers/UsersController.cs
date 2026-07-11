using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.Shared.Validators.Attributes;
using FinalYearProject.Services.UserMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
//[Authorize]
public class UsersController(IUserMgmtService userService) : BaseController
{
    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [SuperAdmin]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserParameters parameters,
        CancellationToken cancellationToken)
    {
        var response = await userService.GetUsersAsync(parameters, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Get details of a particular user
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [SuperAdmin]
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
    [SuperAdmin]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await userService.CreateUserAsync(request, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Update user's basic information
    /// </summary>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await userService.UpdateUserAsync(
            id,
            request,
            cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Update user's attributes
    /// Reserved for Super Admins
    /// </summary>
    [HttpPut("{id}/attributes")]
    [SuperAdmin]
    public async Task<IActionResult> UpdateUserAttributes(
        Guid id,
        [FromBody] UpdateUserAttributesRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await userService.UpdateUserAttributesAsync(
            id,
            request,
            cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Delete a user by their unique identifier
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    [SuperAdmin]
    public async Task<IActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await userService.DeleteUserAsync(id, cancellationToken);
        return ComputeResponse(response);
    }
}
