using FinalYearProject.Data.Context;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Dashboard;

public class DashboardService(
    FileSystemDbContext database)
    : IDashboardService
{
    public async Task<ServiceResponse<DashboardSummaryResponse>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var response = new DashboardSummaryResponse(
                TotalFiles: await database.Uploads.CountAsync(cancellationToken),
                TotalUsers: await database.Users.CountAsync(cancellationToken),
                TotalPolicies: await database.Policies.CountAsync(cancellationToken)
            );

            return Response.Success("Dashboard summary fetched successfully", response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching dashboard summary");

            return Response.SystemMalfunction<DashboardSummaryResponse>(
                "Something went wrong", null!);
        }
    }

    public async Task<ServiceResponse<List<RecentFileResponse>>> GetRecentUploadsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var uploads = await database.Uploads
                .Include(x => x.Policy)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x => new RecentFileResponse(
                    x.Id,
                    x.Filename,
                    x.CreatedBy ?? "Unknown",
                    x.Policy != null
                        ? x.Policy.PolicyName
                        : "No Policy",
                    x.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return Response.Success("Recent uploads fetched successfully", uploads);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching recent uploads");

            return Response.SystemMalfunction<List<RecentFileResponse>>(
                "Something went wrong", null!);
        }
    }
}
