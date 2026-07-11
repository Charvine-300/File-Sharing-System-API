using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.UploadsMgmt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]

public class UploadsController(IUploadsMgmtService uploadsMgmtService) : BaseController
{
    /// <summary>
    /// Get all files in the system with optional filtering and pagination.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<IActionResult> GetFiles(
        [FromQuery] FileParameters parameters,
        CancellationToken cancellationToken)
    {
        var response = await uploadsMgmtService.GetFilesAsync(
            parameters,
            cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Get a specific file by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await uploadsMgmtService.GetFileAsync(
            id,
            cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Upload a new file to the system.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadFile(
        [FromForm] FileMgmtRequest request,
        CancellationToken cancellationToken)
    {
        var response = await uploadsMgmtService.UploadFileAsync(
            request,
            cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Download a file with authorized-access
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id}/download-document")]
    public async Task<IActionResult> DownloadFile(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await uploadsMgmtService.DownloadFileAsync(
            id,
            cancellationToken);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return BadRequest(response);
        }

        return File(
            response.Data.FileBytes,
            response.Data.ContentType,
            response.Data.FileName);
    }

        /// <summary>
        /// Change access policy on encrypted files
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id}/update-policy")]
    public async Task<IActionResult> UpdatePolicy(
    Guid id,
    UpdateFilePolicyRequest request,
    CancellationToken cancellationToken)
    {
        var response =
            await uploadsMgmtService.UpdateFilePolicyAsync(
                id,
                request,
                cancellationToken);


        return ComputeResponse(response);
    }

    /// <summary>
    /// Delete files
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(
    Guid id,
    CancellationToken cancellationToken)
    {
        var response =
            await uploadsMgmtService.DeleteFileAsync(
                id,
                cancellationToken);


        return ComputeResponse(response);
    }
}