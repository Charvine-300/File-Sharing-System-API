using FinalYearProject.Controllers.Shared;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.AuditTrails;
using FinalYearProject.Services.Shared.Validators.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize]
public class AuditLogController(IAuditLogMgmtService auditLogMgmtService) : BaseController
{
    /// <summary>
    /// Get all audit logs
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [SuperAdmin]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] RequestParameters parameters,
        CancellationToken cancellationToken)
    {
        var response = await auditLogMgmtService.GetAuditLogsAsync(parameters, cancellationToken);
        return ComputeResponse(response);
    }


    /// <summary>
    /// Log actions
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [SuperAdmin]
    public async Task<IActionResult> DeleteAuditLog(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await auditLogMgmtService.DeleteAuditLogAsync(id, cancellationToken);
        return ComputeResponse(response);
    }

}

