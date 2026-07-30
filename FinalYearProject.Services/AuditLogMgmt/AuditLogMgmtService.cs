using FinalYearProject.Data.Context;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Utilities;
using FinalYearProject.Services.Shared;
using FinalYearProject.Services.Shared.UserContextService;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinalYearProject.Services.AuditTrails;

public class AuditLogMgmtService(IUserContextService userContextService, FileSystemDbContext database) : IAuditLogMgmtService
{
    public async Task<ServiceResponse<PaginationResponse<AuditTrailResponse>>> GetAuditLogsAsync(RequestParameters parameters, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<AuditLog> query = database.AuditLogs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.ToLower();
                query = query.Where(t => t.Actor.ToLower().Contains(search));
            }

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= parameters.StartDate.Value.Date);
            }

            if (parameters.EndDate.HasValue)
            {
                var end = parameters.EndDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedAt < end);
            }

            int totalCount = await query.CountAsync(cancellationToken);
            int totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

            List<AuditLog> trail = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            List<AuditTrailResponse> data = trail.Select(cr => new AuditTrailResponse
        (
            cr.Id,
            cr.Action,
            cr.Description,
            cr.Actor,
            cr.ActionType.ToString(),
            cr.ActorType.ToString(),
            cr.CreatedAt
        )).ToList();


            return Response.Success("Audit Trail retrieved successfully.", new PaginationResponse<AuditTrailResponse>
            (
                Records: data,
                TotalRecords: totalCount,
                TotalPages: totalPages,
                CurrentPage: parameters.PageNumber,
                PageSize: parameters.PageSize
            ));
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error retrieving audit trails for SecondaryIssuerId: {SecondaryIssuerId}", null!);
            return Response.SystemMalfunction<PaginationResponse<AuditTrailResponse>>("An error occurred while retrieving audit trails.", null!);
        }
    }

    public async Task CreateAuditLogAsync(CreateAuditTrailRequest request, CancellationToken cancellationToken)
    {
        AuditLog trail = new()
        {
            Action = request.Action,
            Description = request.Description,
            Actor = request.Actor,
            ActionType = request.ActionType,
            ActorType = userContextService.User.UserType,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userContextService.User.FirstName
        };

        database.AuditLogs.Add(trail);
    }

    public async Task<ServiceResponse> DeleteAuditLogAsync(Guid auditLogId, CancellationToken cancellationToken)
    {
        try
        {
            AuditLog? trail = await database.AuditLogs
            .FirstOrDefaultAsync(r => r.Id == auditLogId);
            if (trail is null)
            {
                return Response.NotFound("Audit Log not found.");
            }


            database.AuditLogs.Remove(trail);
            await database.SaveChangesAsync(cancellationToken);

            return Response.Success("Audit Log deleted successfully.");
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            Log.Logger.Error(ex, "Error deleting audit log with ID: {AuditLogId} for SecondaryIssuerId: {SecondaryIssuerId}", auditLogId, null!);
            return Response.SystemMalfunction("An error occurred while deleting the audit log.");
        }
    }
}
