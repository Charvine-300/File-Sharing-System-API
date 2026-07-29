using FinalYearProject.Data.Utilities;

namespace FinalYearProject.Services.AuditTrails;

public interface IAuditLogMgmtService
{
    Task<ServiceResponse<PaginationResponse<AuditTrailResponse>>> GetAuditLogsAsync(RequestParameters parameters, CancellationToken cancellationToken);
    Task CreateAuditLogAsync(CreateAuditTrailRequest request , CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAuditLogAsync(Guid auditLogId, CancellationToken cancellationToken);
}
