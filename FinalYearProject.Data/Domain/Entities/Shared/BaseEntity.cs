namespace FinalYearProject.Data.Domain.Entities.Shared;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public bool Deleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
}
