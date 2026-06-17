using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.AttributeMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/attributes")]
//[Authorize]
public class AttributesController(IAttributeMgmtService attributeService) : BaseController
{
    /// <summary>
    /// Get all attributes with pagination and filtering by name and type.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAttributes(
        [FromQuery] AttributeParameters parameters,
        CancellationToken cancellationToken)
    {
        var response = await attributeService.GetAttributesAsync(parameters, cancellationToken);
        return ComputeResponse(response);
    }


    /// <summary>
    /// Get all details of a particular attribute
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttributeDetails(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await attributeService.GetAttributeDetailsAsync(id, cancellationToken);
        return ComputeResponse(response);
    }


    /// <summary>
    /// Create a new attribute
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateAttribute(
        [FromBody] AttributeMgmtRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await attributeService.CreateAttributeAsync(request, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Update an existing attribute
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAttribute(
        Guid id,
        [FromBody] AttributeMgmtRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await attributeService.UpdateAttributeAsync(id, request, cancellationToken);
        return ComputeResponse(response);
    }

    /// <summary>
    /// Delete an existing attribute
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAttribute(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await attributeService.DeleteAttributeAsync(id, cancellationToken);
        return ComputeResponse(response);
    }
}