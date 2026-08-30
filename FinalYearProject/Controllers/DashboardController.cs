using FinalYearProject.Controllers.Shared;
using FinalYearProject.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : BaseController
{
    /// <summary>
    /// Gets dashboard statistics.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        CancellationToken cancellationToken)
    {
        var response =
            await dashboardService.GetDashboardSummaryAsync(
                cancellationToken);

        return ComputeResponse(response);
    }

    /// <summary>
    /// Gets the most recently uploaded files.
    /// </summary>
    [HttpGet("recent-files")]
    public async Task<IActionResult> GetRecentFiles(
        CancellationToken cancellationToken)
    {
        var response =
            await dashboardService.GetRecentUploadsAsync(
                cancellationToken);

        return ComputeResponse(response);
    }
}
