using FinalYearProject.Data.Domain.Entities.Shared;


namespace FinalYearProject.Data.Domain.Entities;

public class AuditLog: BaseEntity
{
    public string Action { get; set; }
    public string? Description { get; set; }
    public string Actor { get; set; }
    public ActionType ActionType { get; set; }
    public UserType ActorType { get; set; }
}
