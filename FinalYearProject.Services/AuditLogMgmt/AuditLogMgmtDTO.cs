using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Services.AuditTrails;

public record AuditTrailResponse(Guid Id, string Action, string? Description, string Actor, string ActionType, string ActorType, DateTimeOffset CreatedAt);

public class CreateAuditTrailRequest
{
    public string Action { get; set; } = null!;
    public string? Description { get; set; }
    public string Actor { get; set; }
    public ActionType ActionType { get; set; }
}

