using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.PolicyMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/policies")]
[Authorize]
//[Authorize]
public class PoliciesController(IPolicyMgmtService policyService) : BaseController
{

    /// <summary>
    /// Create a new CP-ABE policy
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreatePolicy(
        [FromBody] CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response =
            await policyService.CreatePolicyAsync(
                request,
                cancellationToken);

        return ComputeResponse(response);
    }



    /// <summary>
    /// Get a single policy by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPolicy(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await policyService.GetPolicyAsync(
                id,
                cancellationToken);

        return ComputeResponse(response);
    }



    /// <summary>
    /// Get all available policies
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPolicies(
        [FromQuery] PolicyParameters parameters,
        CancellationToken cancellationToken)
    {
        var response =
            await policyService.GetPoliciesAsync(
                parameters,
                cancellationToken);

        return ComputeResponse(response);
    }



    /// <summary>
    /// Update an existing policy
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePolicy(
        Guid id,
        [FromBody] UpdatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response =
            await policyService.UpdatePolicyAsync(
                id,
                request,
                cancellationToken);

        return ComputeResponse(response);
    }



    /// <summary>
    /// Disable a policy
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePolicy(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response =
            await policyService.DeletePolicyAsync(
                id,
                cancellationToken);

        return ComputeResponse(response);
    }
}