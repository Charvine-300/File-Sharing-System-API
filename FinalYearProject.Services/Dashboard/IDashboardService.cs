using FinalYearProject.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Dashboard;

public interface IDashboardService
{
    Task<ServiceResponse<DashboardSummaryResponse>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken);

    Task<ServiceResponse<List<RecentFileResponse>>> GetRecentUploadsAsync(
        CancellationToken cancellationToken);
}
