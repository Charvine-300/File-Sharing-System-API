using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Dashboard;

public record DashboardSummaryResponse(
    int TotalFiles,
    int TotalUsers,
    int TotalPolicies
);

public record RecentFileResponse(
    Guid Id,
    string FileName,
    string UploadedBy,
    string Policy,
    DateTimeOffset UploadedAt
);