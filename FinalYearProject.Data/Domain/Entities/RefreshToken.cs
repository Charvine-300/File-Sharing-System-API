using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Data.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Timestamp]
    public byte[] TimeStamp { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    public string Token { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
    public string? Roles { get; set; }
}

